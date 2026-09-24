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

    public async Task<IReadOnlyList<ReservationProfile?>> GetReservationProfiles(CancellationToken cancellationToken)
    {
        return await context.ReservationProfiles
                            .Include(p => p.Reservations)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
    }

    public async Task<Reservation?> GetReservationById(ReservationId reservationId, CancellationToken cancellationToken)
    {
        return await context.Reservations
                            .AsNoTracking()
                            .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
    }

    public async Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId reservationProfileId)
    {
        return await context.ReservationProfiles
                            .Include(p => p.Reservations)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == reservationProfileId);
    }

    public async Task<ReservationProfile?> GetReservationProfileByUserId(UserId userId)
    {
        return await context.ReservationProfiles
                            .Include(p => p.Reservations)
                            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> CheckReservationDates(Guid gameId, DateTimeOffset startDate, DateTimeOffset endDate)
    {

        var invalidDates = await context.Reservations.AsNoTracking()
                                                     .AnyAsync(r => r.GameId == gameId &&
                                                               r.StartDate <= endDate && 
                                                               r.EndDate >= startDate
                                                              );
        return invalidDates;
    }

    public async Task CreateReservation(Reservation reservation)
    {
        context.Reservations.Add(reservation);
    }
}