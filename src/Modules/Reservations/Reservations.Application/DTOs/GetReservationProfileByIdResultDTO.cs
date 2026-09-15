using Reservations.Domain.Aggregates;

namespace Reservations.Application.DTOs;
public record GetReservationProfileByResultId(Guid ReservationProfileId, Guid UserId, List<Reservation> Reservations);