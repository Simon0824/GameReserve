using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record BanUserCommand(string Email) : ICommand<Result>;
public class BanUserCommandHandler(IUserRepository userRepository) : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUserByEmail(request.Email);

        if(user is null)
            return UserErrors.UserNotFound;

        if(user.Status == UserStatus.Banned)
          return UserErrors.UserBanned;

        user.BanUser(user);

        await userRepository.UpdateAsync(user);
        
        return Result.Success;
    }
}