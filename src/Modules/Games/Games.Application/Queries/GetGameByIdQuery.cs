using Games.Application.DTOs;
using Games.Domain.Interfaces;
using MediatR;

namespace Games.Application.Queries;
public record GetGameByIdQuery(Guid Id) : IRequest<GetGamesCatalogResultDTO>;
public class GetGameByIdQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGameByIdQuery, GetGamesCatalogResultDTO>
{
    public async Task<GetGamesCatalogResultDTO> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await gameRepository.FindGameById(request.Id, cancellationToken);

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