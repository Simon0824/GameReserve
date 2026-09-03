using Identity.Application.DTOs;
using Identity.Domain.Entites;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands;
public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResultDTO>;

public class LoginUserCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider) :IRequestHandler<LoginUserCommand, LoginUserResultDTO>
{
    public async Task<LoginUserResultDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUser(request.Email);

        if(user is null)
        {
            throw new Exception("User is not found");
        }

        var isPasswordValid = await userRepository.CheckPassword(user, request.Password);

        if(!isPasswordValid)
        {
            throw new Exception("You've entered a wrong password");
        }

        var token = tokenProvider.CreateToken(user);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
            User = user
        };

        await userRepository.AddRefreshToken(refreshToken);

        return new LoginUserResultDTO(
            user.FullName,
            user.Email!,
            token,
            refreshToken.Token
        );
    }
}
