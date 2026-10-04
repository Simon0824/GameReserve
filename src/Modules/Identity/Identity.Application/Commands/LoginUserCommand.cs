using SharedKernel.Application.Abstractions.Messaging;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;
using Identity.Domain.UserAggregate;
using Identity.Domain.Enums;

namespace Identity.Application.Commands;
public record LoginUserCommand(string Email, string Password) : ICommand<Result<LoginUserResultDTO>>;

public class LoginUserCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider) : IRequestHandler<LoginUserCommand, Result<LoginUserResultDTO>>
{
    public async Task<Result<LoginUserResultDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUserByEmail(request.Email);

        if(user is null)
        {
            return UserErrors.UserNotFound;
        }

        var isPasswordValid = await userRepository.CheckPassword(user, request.Password);

        if(!isPasswordValid)
        {
            return UserErrors.PasswordNotValid;
        }

        if(user.Status == UserStatus.Banned)
            return UserErrors.UserBanned;

        var token = await tokenProvider.CreateToken(user);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
            User = user
        };

        userRepository.AddRefreshToken(refreshToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return new LoginUserResultDTO(
            user.FullName,
            user.Email!,
            token,
            refreshToken.Token
        );
    }
}
