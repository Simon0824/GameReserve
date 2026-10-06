using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Games.Application.Commands;
public record DeleteGameCommand(Guid GameId) : ICommand<Result>;
public class DeleteGameCommandHandler(IGameRepository gameRepository) : IRequestHandler<DeleteGameCommand, Result>
{
    public async Task<Result> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await gameRepository.FindGameById(new GameId(request.GameId), cancellationToken);

        if(game is null)
            return GamesErrors.GameNotFound;

        await gameRepository.DeleteGameAsync(game.Id);
        await gameRepository.SaveChanges(cancellationToken);

        return Result.Success;
    }
}