using MassTransit;
using MassTransit.Serialization;
using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

public record GetReservationByIdQuery(Guid ReservatonId) : IRequest<Result<GetReservationDTO>>;
public class GetReservationByIdQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationByIdQuery,
                                                                                                                  Result<GetReservationDTO>>
{
    public async Task<Result<GetReservationDTO>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await reservationsRepository.GetReservationById(new ReservationId(request.ReservatonId), cancellationToken);

        if(reservation is null)
        {
          return ReservationErrors.ReservationProfileNotFoundById;
        }

        return new GetReservationDTO(
            reservation.Id.Value,
            reservation.GameId,
            reservation.StartDate,
            reservation.EndDate,
            reservation.Status
        );
    }
}