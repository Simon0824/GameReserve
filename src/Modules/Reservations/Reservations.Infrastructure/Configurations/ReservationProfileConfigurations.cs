using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;

namespace Reservations.Infrastructure.Configurations;
public class ReservationProfileConfigurations : IEntityTypeConfiguration<ReservationProfile>
{
    public void Configure(EntityTypeBuilder<ReservationProfile> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Id)
               .HasConversion(
                id => id.Value,
                value => new ReservationProfileId(value)
               );
        
        builder.Property(p => p.UserId)
               .HasConversion(
                userid => userid.Value,
                value => new UserId(value)
               );

        builder.HasIndex(x => x.UserId)
               .IsUnique();

        builder.HasMany(p => p.reservations)
               .WithOne()
               .HasForeignKey("ReservaiotnProfileId");
        
        builder.Navigation(p => p.reservations)
               .HasField("_reservations")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}