using SharedKernel.Domain.Abstractions;

namespace Payments.Domain.Aggregates;
public class PaymentErrors
{
    public static readonly Error PaymentWithExternalIdNotFound = Error.NotFound("Payment.NotFound", "Payment with external ID is not found");
    public static readonly Error PaymentAlreadyFailed = Error.Conflict("Payment.AlreadyFailed", "Payment has already failed");
}