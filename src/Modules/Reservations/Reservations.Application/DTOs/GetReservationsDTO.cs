namespace Reservations.Application.DTOs;
public record GetReservationsDTO(Guid ReservationId, Guid GameId, DateTime StartDate, DateTime EndDate);