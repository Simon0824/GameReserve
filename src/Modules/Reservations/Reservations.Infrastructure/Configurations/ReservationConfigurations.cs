using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Infrastructure.Configurations;
public class ReservationConfigurations : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(r => r.Id)
               .HasConversion(
                id => id.Value,
                value => new ReservationId(value)
               );
        
        builder.Property(r => r.StartDate)
              .HasColumnType("timestamp with time zone");
        
        builder.Property(r => r.EndDate)
               .HasColumnType("timestamp with time zone");

        builder.Property(r => r.Status)
                        .HasConversion<string>();
    }
}