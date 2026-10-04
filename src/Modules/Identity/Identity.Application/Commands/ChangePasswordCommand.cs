using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword) : ICommand<Result>;
public class ChangePasswordCommandHandler(IUserRepository userRepository) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUser(request.UserId);

        if(user is null)
            return UserErrors.UserNotFound;

        var checkPassword = await userRepository.CheckPassword(user, request.CurrentPassword);

        if(checkPassword == false)
            return UserErrors.PasswordNotValid;
        
        var changePassword = await userRepository.ChangePassword(user, request.CurrentPassword, request.NewPassword);

        if(!changePassword.Succeeded)
            return UserErrors.CannotChangePassword;

        return Result.Success;
    }
}