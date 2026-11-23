using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

       var valueConverter = new ValueConverter<ExternalPaymentId?, string?>(
              id => id.HasValue ? id.Value.Value : null,
              value => value == null ? null : new ExternalPaymentId(value)
       );

       builder.Property(p => p.ExternalPaymentId)
              .HasConversion(valueConverter)
              .IsRequired(false);

        builder.HasIndex(p => p.ExternalPaymentId)
               .IsUnique();
    }
}