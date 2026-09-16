using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Domain.Interfaces;
public interface IReservationsRepository
{
    void AddReservationProfile(ReservationProfile reservationProfile);
    Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId reservationProfileId);
    Task<ReservationProfile?> GetReservationProfileByUserId(UserId userId);
    Task CreateReservation(Reservation reservation);
}