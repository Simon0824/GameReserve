using Games.Application.DTOs;
using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Games.Application.Queries;
public record GetGameByIdQuery(Guid Id) : IRequest<Result<GetGamesCatalogResultDTO>>;
public class GetGameByIdQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGameByIdQuery, Result<GetGamesCatalogResultDTO>>
{
    public async Task<Result<GetGamesCatalogResultDTO>> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var gameId = new GameId(request.Id);
        var game = await gameRepository.FindGameById(gameId, cancellationToken);

        if(game is null)
        {
            return Result<GetGamesCatalogResultDTO>.Failure(GamesErrors.GameNotFound);
        }

        return Result<GetGamesCatalogResultDTO>.Succes(new GetGamesCatalogResultDTO(
            game.Id.Value,
            game.Title,
            game.Description
        ));
    }
}