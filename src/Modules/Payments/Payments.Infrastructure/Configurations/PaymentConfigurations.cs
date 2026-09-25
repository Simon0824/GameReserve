using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Aggregates;
using Payments.Domain.Primitives;

namespace Payments.Infrastructure.Configurations;
public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
               .HasConversion<string>();
            
        builder.Property(p => p.Id)
               .HasConversion(
                id => id.Value,
                value => new PaymentId(value)
               );

        builder.Property(p => p.ReservationId)
               .HasConversion(
                id => id.Value,
                value => new ReservationId(value)
               );

        builder.Property(p => p.ExternalPaymentId)
               .HasConversion(
                id => id.Value,
                value => new ExternalPaymentId(value)
               );

        builder.HasIndex(p => p.ExternalPaymentId)
               .IsUnique();
    }
}