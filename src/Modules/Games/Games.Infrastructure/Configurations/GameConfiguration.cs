using Games.Domain.GameAggregate;
using Games.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Infrastructure.Configurations;
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(k => k.GameId);

        builder.Property(g => g.GameId)
               .HasConversion(
                 id => id.Id,
                 value => new GameId(value)
               );

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(1000);
    }
}