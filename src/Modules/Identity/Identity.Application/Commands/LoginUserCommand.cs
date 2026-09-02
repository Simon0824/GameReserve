using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;

namespace Identity.Application.Commands;
public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResultDTO>;

public class LoginUserCommandHandler() :IRequestHandler<LoginUserCommand, LoginUserResultDTO>
{
    public async Task<LoginUserResultDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        
    }
}
