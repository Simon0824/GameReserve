using Payments.Domain.Aggregates;
using Payments.Domain.Primitives;

namespace Payments.Domain.Interfaces;
public interface IPaymentsRepository
{
    void AddPayment(Payment payment);
    Task<Payment?> GetPaymentByExternalId(PaymentId externalPaymentId, CancellationToken cancellationToken);
    Task<bool> SetExternalIdToPayment(PaymentId paymentId, ExternalPaymentId externalPaymentId, CancellationToken cancellationToken);
    Task<bool> FindReservationDuplicate(ReservationId reservationId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}