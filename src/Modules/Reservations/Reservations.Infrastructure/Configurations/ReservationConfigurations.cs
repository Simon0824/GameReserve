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
               .HasConversion(
                d => d.Kind == DateTimeKind.Utc ? d : d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
               );
        
        builder.Property(r => r.EndDate)
               .HasConversion(
                d => d.Kind == DateTimeKind.Utc ? d : d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
               );

        builder.Property(r => r.Status)
                        .HasConversion<string>();
    }
}