using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Infrastructure.Configurations;
public class InboxMessageConfigurations : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages", schema: "reservations");
        
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Content)
               .HasColumnType("jsonb")
               .IsRequired();
    }
}