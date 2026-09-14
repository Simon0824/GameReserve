using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Infrastructure.Data;

namespace Reservations.Infrastructure.Repositories;
public class ReservationsRepository(ReservationsContext context) : IReservationsRepository
{
    public void AddReservationProfile(
        ReservationProfile reservationProfile)
    {
        context.ReservationProfiles.Add(reservationProfile);
    }
}