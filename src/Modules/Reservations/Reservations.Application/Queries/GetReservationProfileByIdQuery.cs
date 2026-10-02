using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Application.Queries;
public record GetReservationProfileByIdQuery(Guid Id) : IRequest<Result<GetReservationProfileByIdResultDTO>>;
public class GetReservationProfileByIdQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfileByIdQuery, 
                                                                                                                     Result<GetReservationProfileByIdResultDTO>>
{
    public async Task<Result<GetReservationProfileByIdResultDTO>> Handle(GetReservationProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await reservationsRepository.GetReservationProfileById(new ReservationProfileId(request.Id), cancellationToken);
        if(profile is null)
        {
           return ReservationErrors.ReservationProfileNotFoundById;
        }

        return new GetReservationProfileByIdResultDTO(
            profile.Id.Value,
            profile.UserId.Value,
            profile.Reservations
        );
    }
}

