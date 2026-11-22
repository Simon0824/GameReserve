using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record BanUserCommand(Guid UserId) : ICommand<Result>;
public class BanUserCommandHandler(IUserRepository userRepository, IBlackListRepository blackListRepository) : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindUser(request.UserId, cancellationToken);

        if(user is null)
            return UserErrors.UserNotFound;
        

        var banResult = user.BanUser();

        if(banResult.IsFailed)
            return banResult;

        var saved = await blackListRepository.BlackListUserAsync(user.Id, string.Empty, TimeSpan.Zero);

        if(!saved)
            return UserErrors.UserFailedToSaveBan;

        await userRepository.RevokeRefreshTokenByUser(user.Id, cancellationToken);

        await userRepository.SaveChangesAsync(cancellationToken);
        
        return Result.Success;
    }
}