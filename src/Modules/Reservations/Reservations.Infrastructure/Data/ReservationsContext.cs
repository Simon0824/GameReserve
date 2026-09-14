using Microsoft.EntityFrameworkCore;
using Reservations.Domain.Aggregates;

namespace Reservations.Infrastructure.Data;
public class ReservationsContext : DbContext
{
    public ReservationsContext(DbContextOptions<ReservationsContext> options) : base(options) {}

    public DbSet<ReservationProfile> ReservationProfiles {get; set;}
}