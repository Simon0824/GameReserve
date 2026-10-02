using Games.Application.DTOs;
using Games.Domain.Enums;
using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;

namespace Games.Application.Commands;
public record AddNewGameCommand(string Title, string Description, GameCategory Category) : ICommand<Result<AddNewGameResultDTO>>;
public class AddNewGameCommandHandler(IGameRepository gameRepository) : IRequestHandler<AddNewGameCommand, Result<AddNewGameResultDTO>>
{
    public async Task<Result<AddNewGameResultDTO>> Handle(AddNewGameCommand request, CancellationToken cancellationToken)
    {
        var game = Game.Create(request.Title, request.Description, request.Category);

        var gameExist = await gameRepository.FindGame(request.Title, cancellationToken);
        if(gameExist is not null)
        {
            return GamesErrors.GameAlreadyExist;
        }

        gameRepository.AddGame(game);
        await gameRepository.SaveChanges(cancellationToken);

        return new AddNewGameResultDTO(
              game.Id.Value,
              game.Title,
              game.Description,
              game.Category
        );
    }
}