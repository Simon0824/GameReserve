namespace Reservations.Application.DTOs;

public record AddNewReservationResultDTO(Guid ReservationId, string GameTitle, DateTime EndDate);