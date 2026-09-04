using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Commands;
public record LoginWithRefreshTokenCommand(string RefreshToken) : IRequest<LoginWithRefreshTokenResultDTO>;

public class LoginWithRefreshTokenCommandHandler(IUserRepository userRepository) : IRequestHandler<LoginWithRefreshTokenCommand, LoginWithRefreshTokenResultDTO>
{
    public async Task<LoginWithRefreshTokenResultDTO> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
    }
}