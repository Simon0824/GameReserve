using Games.Application.DTOs;
using Games.Domain.Interfaces;
using MediatR;

namespace Games.Application.Queries;
public record GetGameByIdQuery(Guid Id) : IRequest<GetGamesCatalogResultDTO>;
public class GetGameByIdQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetGameByIdQuery, GetGamesCatalogResultDTO>
{
    public async Task<GetGamesCatalogResultDTO> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        
    }
}