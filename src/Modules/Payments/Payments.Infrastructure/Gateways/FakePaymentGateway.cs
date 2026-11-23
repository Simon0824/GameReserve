using Payments.Domain.Entities;
using Payments.Domain.Enums;
using Payments.Domain.Iterfaces;
using Payments.Domain.Primitives;
using Stripe;

namespace Payments.Infrastructure.Gateways;
public class FakePaymentGateway : IPaymentGateway
{
    public async Task<string> CreatePaymentIntentAsync(ReservationId reservationId, decimal amount, PaymentCurrency currency, string cardType)
    {
        if(currency == PaymentCurrency.none)
            throw new Exception("You have to type currency");
            
        long longAmount = (long)(amount*100);

        var paymentIntent = FakePaymentIntent.CreateIntent(longAmount, currency, cardType, reservationId);

        return paymentIntent.Id.ToString();
    }
}