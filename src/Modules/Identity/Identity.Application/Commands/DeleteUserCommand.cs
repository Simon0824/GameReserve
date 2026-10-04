using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.DTOs;
public record DeleteUserCommand(string UserId) : ICommand<Result>;
public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUser(request.UserId);

        if(user is null)
            return UserErrors.UserNotFound;
        
        var isDeleted = await userRepository.DeleteUser(user);

        if(!isDeleted.Succeeded)
            return UserErrors.CannotDeleteUser;

        return Result.Success;
    }
}