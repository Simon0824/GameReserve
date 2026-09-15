using Reservations.Domain.Aggregates;

namespace Reservations.Application.DTOs;
public record GetReservationProfileByIdResultDTO(Guid ReservationProfileId, Guid UserId, IReadOnlyList<Reservation> Reservations);