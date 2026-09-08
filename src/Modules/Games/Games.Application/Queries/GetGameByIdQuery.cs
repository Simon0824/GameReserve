using Games.Application.DTOs;
using MediatR;

namespace Games.Application.Queries;
public record GetGameByIdQuery(Guid Id) : IRequest<GetGamesCatalogResultDTO>;
public class GetGameByIdQueryHandler() : IRequestHandler<GetGameByIdQuery, GetGamesCatalogResultDTO>
{
    public async Task<GetGamesCatalogResultDTO> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        
    }
}