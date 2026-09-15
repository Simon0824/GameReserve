using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;

namespace Reservations.Application.Queries;
public record GetReservationProfileByIdQuery(Guid Id) : IRequest<GetReservationProfileByIdResultDTO>;
public class GetReservationProfileByIdQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfileByIdQuery, 
                                                                       GetReservationProfileByIdResultDTO>
{
    public async Task<GetReservationProfileByIdResultDTO> Handle(GetReservationProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await reservationsRepository.GetReservationProfileById(new ReservationProfileId(request.Id));
        if(profile is null)
        {
            throw new Exception($"Reservation profile with ID: {request.Id} does not exist");
        }

        return new GetReservationProfileByIdResultDTO(
            profile.Id.Value,
            profile.UserId.Value,
            profile.reservations
        );
    }
}

