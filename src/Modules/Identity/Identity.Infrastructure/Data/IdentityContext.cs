using Identity.Domain.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Data;
public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options){}

    public DbSet<User> users {get; set;}
    public DbSet<RefreshToken> refreshTokens {get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
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

    private async Task PublishDomainEvents(DbContext context)
    {
        var entities = context.ChangeTracker
                                    .Entries<Entity>()
                                    .Select(entry => entry.Entity)
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