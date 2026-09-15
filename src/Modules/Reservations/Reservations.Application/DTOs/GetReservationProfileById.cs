namespace Reservations.Application.DTOs;
public record GetReservationProfileByIdDTO
{
    public required Guid ReservationProfileId {get; init;}
}