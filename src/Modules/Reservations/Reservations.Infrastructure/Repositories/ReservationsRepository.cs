using Microsoft.EntityFrameworkCore;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using Reservations.Infrastructure.Data;

namespace Reservations.Infrastructure.Repositories;
public class ReservationsRepository(ReservationsContext context) : IReservationsRepository
{
    public void AddReservationProfile(
        ReservationProfile reservationProfile)
    {
        context.ReservationProfiles.Add(reservationProfile);
    }

    public async Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId profileId)
    {
        return await context.ReservationProfiles
                            .Include(p => p.Reservations)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == profileId);
    }
}