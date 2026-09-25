using Payments.Domain.Aggregates;
using Payments.Domain.Primitives;

namespace Payments.Domain.Interfaces;
public interface IPaymentsRepository
{
    void AddPayment(Payment payment);
    Task<Payment> GetPaymentByExternalId(ExternalPaymentId externalPaymentId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}