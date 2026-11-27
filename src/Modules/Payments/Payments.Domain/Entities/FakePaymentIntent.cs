using Payments.Domain.Primitives;
using SharedKernel.Domain.Enums;

namespace Payments.Domain.Entities;
public class FakePaymentIntent
{
    public Guid Id {get; set;}
    public decimal Amount {get; private set;}
    public PaymentCurrency Currency {get; private set;}
    public CardTypes CardType {get; private set;}
    public ReservationId ReservationId {get; set;}

    private FakePaymentIntent(Guid id, decimal amount, PaymentCurrency currency, CardTypes cardType, ReservationId reservationId)
    {
        Id = id;
        Amount = amount;
        Currency = currency;
        CardType = cardType;
        ReservationId = reservationId;
    }

    public static FakePaymentIntent CreateIntent(decimal amount, PaymentCurrency currency, CardTypes cardType, ReservationId reservationId)
        => new FakePaymentIntent(Guid.NewGuid(), amount,currency, cardType, reservationId);
    
}