using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands;
public record LoginWithRefreshTokenCommand(string RefreshToken) : IRequest<LoginWithRefreshTokenResultDTO>;

public class LoginWithRefreshTokenCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider) : IRequestHandler<LoginWithRefreshTokenCommand, LoginWithRefreshTokenResultDTO>
{
    public async Task<LoginWithRefreshTokenResultDTO> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await userRepository.FindRefreshToken(request.RefreshToken);
        if(refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            throw new Exception("Refresh token is not foud or has expired");
        }

        var accessToken = await tokenProvider.CreateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();

        return new LoginWithRefreshTokenResultDTO(
            accessToken,
            refreshToken.Token
        );
    }
}