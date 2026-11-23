using Payments.Domain.Enums;
using Payments.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Payments.Domain.Entities;
public class FakePaymentIntent : Entity<Guid>
{
    public long Amount {get; private set;}
    public PaymentCurrency Currency {get; private set;}
    public string CardType {get; private set;}
    public ReservationId ReservationId {get; set;}

    private FakePaymentIntent(Guid id, long amount, PaymentCurrency currency, string cardType, ReservationId reservationId) : base(id)
    {
        Amount = amount;
        Currency = currency;
        CardType = cardType;
        ReservationId = reservationId;
    }

    public static FakePaymentIntent CreateIntent(long amount, 
                                PaymentCurrency currency, 
                                string cardType, 
                                ReservationId reservationId)  => new FakePaymentIntent(
                                                                               Guid.NewGuid(), 
                                                                               amount,
                                                                               currency, 
                                                                               cardType, 
                                                                               reservationId);
}