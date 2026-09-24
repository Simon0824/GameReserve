using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Domain.Interfaces;
public interface IReservationsRepository
{
    void AddReservationProfile(ReservationProfile reservationProfile);
    Task<Reservation?> GetReservationById(ReservationId reservationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReservationProfile?>> GetReservationProfiles(CancellationToken cancellationToken);
    Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId reservationProfileId);
    Task<ReservationProfile?> GetReservationProfileByUserId(UserId userId);
    Task<bool> CheckReservationDates(Guid gameId, DateTimeOffset startDate, DateTimeOffset endDate);
    Task CreateReservation(Reservation reservation);
}