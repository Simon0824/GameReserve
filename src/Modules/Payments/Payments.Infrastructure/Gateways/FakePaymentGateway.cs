using Payments.Application.Iterfaces;
using Payments.Domain.Entities;
using Payments.Domain.Primitives;
using Payments.Infrastructure.Processors;
using SharedKernel.Domain.Enums;

namespace Payments.Infrastructure.Gateways;
public class FakePaymentGateway(FakePaymentProcessor paymentProcessor) : IPaymentGateway
{
    public async Task<string> CreatePaymentIntentAsync(ReservationId reservationId, PaymentId paymentId, decimal amount, decimal calculatedPrice, PaymentCurrency currency, CardTypes cardType)
    {
        var paymentIntent = FakePaymentIntent.CreateIntent(amount, currency, cardType, reservationId);

        await paymentProcessor.ExecuteAsync(paymentIntent, paymentId);

        return paymentIntent.Id.ToString();
    }
}