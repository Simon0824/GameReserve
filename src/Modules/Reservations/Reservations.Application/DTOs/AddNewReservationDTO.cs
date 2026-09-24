namespace Reservations.Application.DTOs;

public record AddNewReservationDTO
{
    public required Guid GameId {get; init;}
    public required DateTime StartDate {get; init;}
    public required DateTime EndDate {get; init;}
}