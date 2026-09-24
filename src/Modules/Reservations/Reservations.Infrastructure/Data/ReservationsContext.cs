using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

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

public class PublishDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if(eventData.Context is not null)
        {
            await PublishDomainEvents(eventData.Context);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public async Task PublishDomainEvents(DbContext context)
    {
        var entities = context.ChangeTracker
                              .Entries<Entity<ReservationProfileId>>()
                              .Select(entries => entries.Entity)
                              .Where(entity => entity.DomainEvents.Count > 0)
                              .ToList();
        var domainEvents = entities.SelectMany(entity => entity.DomainEvents).ToList();

        foreach(var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }

        foreach(var entity in entities)
        {
            entity.ClearDomainEvents();
        }
    }
}