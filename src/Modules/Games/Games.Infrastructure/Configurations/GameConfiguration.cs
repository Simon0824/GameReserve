using Games.Domain.GameAggregate;
using Games.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(g => g.Id)
               .HasConversion(
                 id => id.Value,
                 value => new GameId(value)
               );

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Ignore(g => g.DomainEvents);
    }
}