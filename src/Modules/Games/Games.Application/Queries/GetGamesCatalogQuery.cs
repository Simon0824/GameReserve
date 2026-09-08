using Games.Application.DTOs;
using Games.Domain.Interfaces;
using MediatR;

namespace Games.Application.Queries;
public record GetGamesCatalogQuery() : IRequest<List<GetGamesCatalogResultDTO>>;
public class GetGamesCatalogQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGamesCatalogQuery, List<GetGamesCatalogResultDTO>>
{
    public async Task<List<GetGamesCatalogResultDTO>> Handle(GetGamesCatalogQuery request, CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetGames();
        var result = new List<GetGamesCatalogResultDTO>();

        if(games is null || !games.Any())
        {
            return result;
        }

        foreach(var game in games)
        {
            result.Add(new GetGamesCatalogResultDTO(
                game.GameId.Id,
                game.Title,
                game.Description
            ));
        }

        return result;
    }
}