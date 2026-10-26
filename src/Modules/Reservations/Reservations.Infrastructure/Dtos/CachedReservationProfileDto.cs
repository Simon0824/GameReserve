using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Infrastructure.Dtos;
public record CachedReservationProfileDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<CachedReservationDto> Reservations
);