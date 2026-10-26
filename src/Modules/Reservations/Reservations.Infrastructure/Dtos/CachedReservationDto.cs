using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Reservations.Infrastructure.Dtos;
public record CachedReservationDto(
    Guid Id,
    Guid ReservationProfileId,
    Guid GameId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string Status
);