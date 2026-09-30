using Payments.Domain.Enums;
using Payments.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Payments.Domain.Aggregates;
public class Payment : Entity<PaymentId>
{
    public ReservationId ReservationId {get; private set;}
    public ExternalPaymentId ExternalPaymentId {get; private set;}
    public decimal Amount {get; private set;}
    public PaymentStatus Status {get; private set;}

    public Payment(PaymentId paymentId, ExternalPaymentId externalPaymentId, ReservationId reservationId, decimal amount)
    {
        Id = paymentId;
        ExternalPaymentId = externalPaymentId;
        ReservationId = reservationId;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    public static Payment CreatePayment(ReservationId reservationId, string externalPaymentId, decimal amount) =>
                                                            new Payment(new PaymentId(Guid.NewGuid()), new ExternalPaymentId(externalPaymentId), reservationId, amount);

    public Result Complete()
    {
        if(Status == PaymentStatus.Completed)
        {
            return Result.Succes;
        }
        else if(Status == PaymentStatus.Failed)
        {
            return Result.Failure(PaymentErrors.PaymentAlreadyFailed);
        }

        Status = PaymentStatus.Completed;
        return Result.Succes;
    }

    public void Failed()
    {
        Status = PaymentStatus.Failed;
    }

    private Payment() {}
}