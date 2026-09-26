
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using Payments.Infrastructure.Data;

namespace Payments.Infrastructure.Repositories;
public class PaymentsRepository(PaymentsContext context) : IPaymentsRepository
{
    public void AddPayment(Payment payment)
    {
        context.Payments.Add(payment);
    }

    public async Task<Payment?> GetPaymentByExternalId(ExternalPaymentId externalPaymentId, CancellationToken cancellationToken)
    {
        return await context.Payments
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                p => p.ExternalPaymentId == externalPaymentId, 
                                                            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}