using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Commands;
public record LogoutUserCommand(string TokenId, string ExpiresOnUtc) : ICommand<Result>;
public class LogoutUserCommandHandler(IBlackListRepository blackListRepository) : IRequestHandler<LogoutUserCommand, Result>
{
    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var timeLeft = DateTimeOffset.FromUnixTimeSeconds(long.Parse(request.ExpiresOnUtc)).UtcDateTime - DateTime.UtcNow;

        var saved = await blackListRepository.BlackListUserAsync(null, request.TokenId, timeLeft);

        if(!saved)
            return UserErrors.UserFailedToSaveBan;

        return Result.Success;
    }
}