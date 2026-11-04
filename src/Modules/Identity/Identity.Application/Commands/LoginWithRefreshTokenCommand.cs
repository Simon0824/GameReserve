using SharedKernel.Application.Abstractions.Messaging;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;
using Identity.Domain.UserAggregate;
using Identity.Domain.Enums;
using Identity.Domain.Entities;

namespace Identity.Application.Commands;
public record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<Result<LoginWithRefreshTokenResultDTO>>;

public class LoginWithRefreshTokenCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider) : 
                                                                                            IRequestHandler<LoginWithRefreshTokenCommand, 
                                                                                                Result<LoginWithRefreshTokenResultDTO>>
{
    public async Task<Result<LoginWithRefreshTokenResultDTO>> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await userRepository.FindRefreshToken(request.RefreshToken, cancellationToken);
        if(refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            return UserErrors.CannotLogInRefreshToken;
        }

        var user = await userRepository.FindUser(refreshToken.UserId, cancellationToken);

        if(user is null)
            return UserErrors.UserNotFound;

        if(user.Status == UserStatus.Banned)
            return UserErrors.UserBanned;

        var accessToken = await tokenProvider.CreateToken(refreshToken.User);

        refreshToken.ExpiresOnUtc = DateTime.UtcNow;

        var newRefreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
            User = user
        };

        userRepository.AddRefreshToken(newRefreshToken);

        await userRepository.SaveChangesAsync(cancellationToken);

        return new LoginWithRefreshTokenResultDTO(
            accessToken,
            newRefreshToken.Token
        );
    }
}