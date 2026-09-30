using SharedKernel.Domain.Abstractions;

namespace Payments.Domain.Aggregates;
public class PaymentErrors
{
    public static readonly Error PaymentWithExternalIdNotFound = new("Payment.NotFound", "Payment with external ID is not found");
    public static readonly Error PaymentAlreadyFailed = new("Payment.AlreadyFailed", "Payment has already failed");
}