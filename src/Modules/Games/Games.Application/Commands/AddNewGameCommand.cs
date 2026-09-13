using Games.Application.DTOs;
using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using MediatR;
using SharedKernel.Application.Abstractions.Messaging;

namespace Games.Application.Commands;
public record AddNewGameCommand(string Title, string Description) : ICommand<AddNewGameResultDTO>;
public class AddNewGameCommandHandler(IGameRepository gameRepository) : IRequestHandler<AddNewGameCommand, AddNewGameResultDTO>
{
    public async Task<AddNewGameResultDTO> Handle(AddNewGameCommand request, CancellationToken cancellationToken)
    {
        var game = Game.Create(request.Title, request.Description);

        var gameExist = await gameRepository.FindGame(request.Title, cancellationToken);
        if(gameExist is not null)
        {
            throw new Exception("Game is already in database");
        }

        await gameRepository.AddGame(game, cancellationToken);
        await gameRepository.SaveChanges(cancellationToken);

        return new AddNewGameResultDTO(
              game.GameId.Id,
              game.Title,
              game.Description
        );
    }
}