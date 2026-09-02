using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;

namespace Identity.Application.Commands;
public record CreateUserCommand(string FullName, string Email, string Password) : IRequest<CreateUserResultDTO>;

public class CreateUserCommandHandler(IUserRepository userRepository, IPublisher publisher) : IRequestHandler<CreateUserCommand, CreateUserResultDTO>
{
    public async Task<CreateUserResultDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.FullName, request.Email);
        var result = await userRepository.CreateUser(user, request.Password);
        if(!result.Succeeded)
        {
            throw new Exception("Cannot create a user");
        }

        var roleResult = await userRepository.AddUserRole(user);

        if(!roleResult.Succeeded)
        {
            var deleteResult = await userRepository.DeleteUser(user);
            
            if(!deleteResult.Succeeded)
            {
                throw new Exception("Cannot add role to user and failed to delete user");
            }

            throw new Exception("Cannot add role to user");
        }

        foreach(var domainEvent in user.DomainEvents)
        {
            await publisher.Publish(domainEvent);
        }

        return new CreateUserResultDTO(
                user.Id,
                user.FullName,
                user.Email!
        );
    }
}