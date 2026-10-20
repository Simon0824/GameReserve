using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Infrastructure.Configurations;
public class OutboxMessagesConfigurations : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", schema: "reservations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Content)
               .HasColumnType("jsonb")
               .IsRequired();
    }
}