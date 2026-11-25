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

    public async Task<Payment?> GetPaymentByExternalId(PaymentId paymentId, CancellationToken cancellationToken)
    {
        return await context.Payments
                            .FirstOrDefaultAsync(
                                p => p.Id == paymentId, cancellationToken);
    }

    public async Task<bool> FindReservationDuplicate(ReservationId reservationId, CancellationToken cancellationToken)
    {
        return await context.Payments
                            .AnyAsync(
                                p => p.ReservationId == reservationId, cancellationToken);
    }

    public async Task<bool> SetExternalIdToPayment(PaymentId paymentId, ExternalPaymentId externalPaymentId, CancellationToken cancellationToken)
    {
        var affected = await context.Payments.Where(p => p.Id == paymentId)
                                             .ExecuteUpdateAsync(p => 
                                             p.SetProperty(p => p.ExternalPaymentId, externalPaymentId), cancellationToken);
        return affected > 0;
    }


    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}