using MassTransit;
using MassTransit.Serialization;
using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;

public record GetReservationByIdQuery(Guid ReservatonId) : IRequest<GetReservationDTO>;
public class GetReservationByIdQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationByIdQuery,
                                                                                                                  GetReservationDTO>
{
    public async Task<GetReservationDTO> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await reservationsRepository.GetReservationById(new ReservationId(request.ReservatonId), cancellationToken);

        if(reservation is null)
        {
          throw new Exception($"Cannot find reservation with ID: {request.ReservatonId}");
        }

        return new GetReservationDTO(
            reservation.Id.Value,
            reservation.GameId,
            reservation.StartDate,
            reservation.EndDate
        );
    }
}