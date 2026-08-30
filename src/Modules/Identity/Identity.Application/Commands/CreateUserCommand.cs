using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;

namespace Identity.Application.Commands;
public record CreateUserCommand(string FullName, string Email, string Password) : IRequest<UserCreateResultDTO>;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, UserCreateResultDTO>
{
    public async Task<UserCreateResultDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.FullName, request.Email);
        var result = await userRepository.CreateUser(user, request.Password);
        if(!result.Succeeded)
        {
            throw new Exception("Cannot create a user");
        }

        return new UserCreateResultDTO(
                user.Id,
                user.FullName,
                user.Email!
        );
    }
}