using Identity.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Configurations;
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Email).IsRequired();

        builder.Property(p => p.PasswordHash).IsRequired();

        builder.Property(p => p.FullName).HasMaxLength(100).IsRequired();

        builder.Property(p => p.Status)
               .HasConversion<string>();
    }
}