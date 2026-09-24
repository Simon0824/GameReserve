namespace Reservations.Application.DTOs;
public record GetReservationDTO(Guid ReservationId, Guid GameId, DateTimeOffset StartDate, DateTimeOffset EndDate);