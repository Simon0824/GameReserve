
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Aggregates;

namespace Payments.Infrastructure.Data;
public class PaymentsContext(DbContextOptions<PaymentsContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsContext).Assembly);
    }
}