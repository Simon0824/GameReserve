using Microsoft.EntityFrameworkCore;
using Reservations.Domain.Aggregates;

namespace Reservations.Infrastructure.Data;
public class ReservationsContext : DbContext
{
    public ReservationsContext(DbContextOptions<ReservationsContext> options) : base(options) {}

    public DbSet<ReservationProfile> ReservationProfiles {get; set;}
    public DbSet<Reservation> Reservations {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsContext).Assembly);
    }
}