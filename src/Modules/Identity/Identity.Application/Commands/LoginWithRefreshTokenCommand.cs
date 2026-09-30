using SharedKernel.Application.Abstractions.Messaging;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;
using Identity.Domain.UserAggregate;

namespace Identity.Application.Commands;
public record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<Result<LoginWithRefreshTokenResultDTO>>;

public class LoginWithRefreshTokenCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider) : 
                                                                                            IRequestHandler<LoginWithRefreshTokenCommand, 
                                                                                                Result<LoginWithRefreshTokenResultDTO>>
{
    public async Task<Result<LoginWithRefreshTokenResultDTO>> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await userRepository.FindRefreshToken(request.RefreshToken);
        if(refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            return Result<LoginWithRefreshTokenResultDTO>.Failure(UserErrors.CannotLogInRefreshTokenException);
        }

        var accessToken = await tokenProvider.CreateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();

        await userRepository.SaveChangesAsync();

        return Result<LoginWithRefreshTokenResultDTO>.Succes(new LoginWithRefreshTokenResultDTO(
            accessToken,
            refreshToken.Token
        ));
    }
}