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


        builder.HasMany(p => p.Reservations)
               .WithOne()
               .HasForeignKey(r => r.ReservationProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Reservations)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);
    }
}