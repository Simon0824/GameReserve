using Reservations.Domain.Aggregates;
namespace Reservations.Application.DTOs;
public record GetReservationProfilesResultDTO(string Message, IReadOnlyList<ReservationProfile?> ReservationProfiles);