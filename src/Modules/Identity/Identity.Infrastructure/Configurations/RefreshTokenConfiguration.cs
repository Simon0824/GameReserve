using Identity.Domain.Entites;
using Identity.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Configurations;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Token).HasMaxLength(200);
        builder.HasIndex(p => p.Token).IsUnique();

        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId);
    }
}