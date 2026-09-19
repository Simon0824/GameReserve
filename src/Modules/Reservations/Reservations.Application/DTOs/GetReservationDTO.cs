namespace Reservations.Application.DTOs;
public record GetReservationDTO(Guid ReservationId, Guid GameId, DateTime StartDate, DateTime EndDate);