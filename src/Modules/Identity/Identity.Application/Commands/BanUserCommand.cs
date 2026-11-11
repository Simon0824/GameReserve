using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record BanUserCommand(string Email, string Reason) : ICommand<Result>;
public class BanUserCommandHandler(IUserRepository userRepository, IBlackListRepository blackListRepository) : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUserByEmail(request.Email, cancellationToken);

        if(user is null)
            return UserErrors.UserNotFound;

        var saved = await blackListRepository.BlockUserPermamentAsync(user.Id);

        if(!saved)
            return UserErrors.UserFailedToSaveBan;

        var banResult = user.BanUser();

        if(banResult.IsFailed)
            return banResult;

        await userRepository.RevokeRefreshTokenByUser(user.Id, cancellationToken);

        await userRepository.SaveChangesAsync(cancellationToken);
        
        return Result.Success;
    }
}