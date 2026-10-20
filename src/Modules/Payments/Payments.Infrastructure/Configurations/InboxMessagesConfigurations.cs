using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Abstractions;

namespace Payments.Infrastructure.Configurations;
public class InboxMessagesConfigurations : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages", schema: "payments");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Content)
               .HasColumnType("jsonb")
               .IsRequired();
    }
}