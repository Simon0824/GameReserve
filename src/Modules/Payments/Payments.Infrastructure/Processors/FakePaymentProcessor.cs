using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Commands;
using Payments.Domain.Entities;
using Payments.Domain.Enums;
using Payments.Domain.Primitives;

namespace Payments.Infrastructure.Processors;
public class FakePaymentProcessor(IPublisher publisher, ILogger<FakePaymentProcessor> logger)
{
    public async Task ExecuteAsync(FakePaymentIntent fakePayment, PaymentId paymentId)
    {
        if(!Enum.TryParse(fakePayment.CardType.ToString(), out CardTypes cardType))
        {
            logger.LogError($"Payment has failed because of invalid card type: {fakePayment.CardType}");

            await publisher.Publish(new FailedPaymentCommand(paymentId.Value.ToString()));
        }

        if(!Enum.TryParse(fakePayment.Currency.ToString(), out PaymentCurrency paymentCurrency))
        {
            logger.LogError($"Payment has failed because of invalid currency: {fakePayment.Currency}");
            
            await publisher.Publish(new FailedPaymentCommand(paymentId.Value.ToString()));
        }

        await publisher.Publish(new ConfirmPaymentCommand(paymentId.Value.ToString()));
    }
}