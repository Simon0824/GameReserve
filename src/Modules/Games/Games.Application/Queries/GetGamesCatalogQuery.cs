using Games.Application.DTOs;
using Games.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Games.Application.Queries;
public record GetGamesCatalogQuery() : IRequest<Result<GetGamesQueryDTO>>;
public class GetGamesCatalogQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGamesCatalogQuery, Result<GetGamesQueryDTO>>
{
    public async Task<Result<GetGamesQueryDTO>> Handle(GetGamesCatalogQuery request, CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetGames();
        var result = new List<GetGamesCatalogResultDTO>();

        var message = games.Count == 0 ?
                            "No games found" : "Games retrieved succesfully";

        if(games.Count == 0) return Result<GetGamesQueryDTO>.Succes(new GetGamesQueryDTO(message, Enumerable.Empty<GetGamesCatalogResultDTO>()));

        foreach(var game in games)
        {
            result.Add(new GetGamesCatalogResultDTO(
                game.Id.Value,
                game.Title,
                game.Description
            ));
        }

        return Result<GetGamesQueryDTO>.Succes(new GetGamesQueryDTO(message, result));
    }
}