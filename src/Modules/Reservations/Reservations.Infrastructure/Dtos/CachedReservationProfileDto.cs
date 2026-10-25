using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Infrastructure.Dtos;
public record CachedReservationProfileDto(
    ReservationProfileId Id,
    UserId UserId,
    IReadOnlyList<Reservation> Reservations
);