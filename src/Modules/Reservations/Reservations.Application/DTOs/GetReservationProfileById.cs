namespace Reservations.Application.DTOs;
public record GetReservationProfileById
{
    public required Guid ReservationProfileId {get; init;}
}