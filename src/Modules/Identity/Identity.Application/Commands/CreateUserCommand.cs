using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;

namespace Identity.Application.Commands;
public record CreateUserCommand(string FullName, string Email, string Password) : IRequest<CreateUserResultDTO>;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, CreateUserResultDTO>
{
    public async Task<CreateUserResultDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.FullName, request.Email);
        var result = await userRepository.CreateUser(user, request.Password);
        if(!result.Succeeded)
        {
            throw new Exception("Cannot create a user");
        }

        return new CreateUserResultDTO(
                user.Id,
                user.FullName,
                user.Email!
        );
    }
}