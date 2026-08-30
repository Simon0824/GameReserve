using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands;
public record UserCreateCommand(string FullName, string Email, string Password) : IRequest<UserCreateResultDTO>;

public class UserCreateCommandHandler(UserCreateCommand request, CancellationToken cancellationToken) : IRequestHandler<UserCreateCommand, UserCreateResultDTO>
{
    public async Task<UserCreateResultDTO> Handle()
    {
        
    }
}