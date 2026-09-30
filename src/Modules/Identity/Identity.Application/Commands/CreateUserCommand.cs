using SharedKernel.Application.Abstractions.Messaging;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record CreateUserCommand(string FullName, string Email, string Password) : ICommand<Result<CreateUserResultDTO>>;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Result<CreateUserResultDTO>>
{
    public async Task<Result<CreateUserResultDTO>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.FullName, request.Email);
        var result = await userRepository.CreateUser(user, request.Password);
        if(!result.Succeeded)
        {
            return Result<CreateUserResultDTO>.Failure(UserErrors.CannotCreateUserException);
        }

        var roleResult = await userRepository.AddUserRole(user);

        if(!roleResult.Succeeded)
        {
            var deleteResult = await userRepository.DeleteUser(user);
            
            if(!deleteResult.Succeeded)
            {
                return Result<CreateUserResultDTO>.Failure(UserErrors.CannotDeleteUserException);
            }

            return Result<CreateUserResultDTO>.Failure(UserErrors.CannotAddUserRoleException);
        }

        return Result<CreateUserResultDTO>.Succes(new CreateUserResultDTO(
                user.Id,
                user.FullName,
                user.Email!
        ));
    }
}