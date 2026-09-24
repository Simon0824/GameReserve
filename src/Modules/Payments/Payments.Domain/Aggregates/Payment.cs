using Payments.Domain.Enums;
using Payments.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Payments.Domain.Aggregates;
public class Payment : Entity<PaymentId>
{
    public ReservationId ReservationId {get; private set;}
    public decimal Amount {get; private set;}
    public PaymentStatus Status {get; private set;}

    public Payment(PaymentId paymentId, ReservationId reservationId, decimal amount)
    {
        Id = paymentId;
        ReservationId = reservationId;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    public static Payment CreatePayment(ReservationId reservationId, decimal amount) =>
                                                            new Payment(new PaymentId(Guid.NewGuid()), reservationId, amount);

    public void Complete()
    {
        if(Status == PaymentStatus.Completed)
        {
            return;
        }
        else if(Status == PaymentStatus.Failed)
        {
            throw new Exception("Payment already has failed");
        }

        Status = PaymentStatus.Completed;
    }

    public void Failed()
    {
        Status = PaymentStatus.Failed;
    }

    private Payment() {}
}