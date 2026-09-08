using Games.Application.DTOs;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using MediatR;

namespace Games.Application.Queries;
public record GetGameByIdQuery(Guid Id) : IRequest<GetGamesCatalogResultDTO>;
public class GetGameByIdQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGameByIdQuery, GetGamesCatalogResultDTO>
{
    public async Task<GetGamesCatalogResultDTO> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var gameId = new GameId(request.Id);
        var game = await gameRepository.FindGameById(gameId, cancellationToken);

        if(game is null)
        {
            throw new Exception($"Game not found, id: {request.Id}");
        }

        return new GetGamesCatalogResultDTO(
            game.GameId.Id,
            game.Title,
            game.Description
        );
    }
}