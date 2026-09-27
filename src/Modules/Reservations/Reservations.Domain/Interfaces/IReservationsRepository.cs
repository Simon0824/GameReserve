using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Domain.Interfaces;
public interface IReservationsRepository
{
    void AddReservationProfile(ReservationProfile reservationProfile);
    Task<Reservation?> GetReservationById(ReservationId reservationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReservationProfile?>> GetReservationProfiles(CancellationToken cancellationToken);
    Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId reservationProfileId, CancellationToken cancellationToken);
    Task<ReservationProfile?> GetReservationProfileByUserId(UserId userId, CancellationToken cancellationToken);
    Task<bool> CheckReservationDates(Guid gameId, DateTimeOffset startDate, DateTimeOffset endDate);
    void CreateReservation(Reservation reservation);
}