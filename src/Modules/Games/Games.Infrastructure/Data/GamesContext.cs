using Games.Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Data;
public class GamesContext : DbContext
{
    public GamesContext(DbContextOptions<GamesContext> options) : base(options) {}

    public DbSet<Game> games {get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(GamesContext).Assembly);
    }
}