using Games.Application.DTOs;
using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using MediatR;

namespace Games.Application.Commands;
public record AddNewGameCommand(string Title, string Description) : IRequest<AddNewGameResultDTO>;
public class AddNewGameCommandHandler(IGameRepository gameRepository) : IRequestHandler<AddNewGameCommand, AddNewGameResultDTO>
{
    public async Task<AddNewGameResultDTO> Handle(AddNewGameCommand request, CancellationToken cancellationToken)
    {
        var game = Game.Create(request.Title, request.Description);

        var isAlreadyInDb = await gameRepository.FindGame(request.Title);
        if(isAlreadyInDb is not null)
        {
            throw new Exception("Game is already in database");
        }

        await gameRepository.AddGame(game);
        await gameRepository.SaveChanges(cancellationToken);

        return new AddNewGameResultDTO(
              game.GameId.ToString(),
              game.Title,
              game.Description
        );
    }
}