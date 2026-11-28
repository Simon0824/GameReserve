using SharedKernel.Domain.Enums;

namespace Reservations.Application.DTOs;

public record AddNewReservationDTO
{
    public required Guid GameId {get; init;}
    public required DateTime StartDate {get; init;}
    public required DateTime EndDate {get; init;}
    public required CardTypes CardType {get; init;}
    public required PaymentCurrency Currency {get; init;}
    public required decimal Amount {get; init;}
}