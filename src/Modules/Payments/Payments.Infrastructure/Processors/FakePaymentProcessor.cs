using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Commands;
using Payments.Domain.Entities;
using Payments.Domain.Enums;
using Payments.Domain.Primitives;

namespace Payments.Infrastructure.Processors;
public class FakePaymentProcessor(ISender sender, ILogger<FakePaymentProcessor> logger)
{
    public async Task ExecuteAsync(FakePaymentIntent fakePayment, PaymentId paymentId)
    {
        if(!Enum.TryParse(fakePayment.CardType.ToString(), out CardTypes cardType))
        {
            logger.LogError($"Payment has failed because of invalid card type: {fakePayment.CardType}");

            var isFailed = await sender.Send(new FailedPaymentCommand(paymentId.Value.ToString()));
            if(isFailed.IsFailed)
                throw new Exception($"Cannot proccess failed payment: {isFailed.Error.Code}");
        }

        if(!Enum.TryParse(fakePayment.Currency.ToString(), out PaymentCurrency paymentCurrency))
        {
            logger.LogError($"Payment has failed because of invalid currency: {fakePayment.Currency}");
            
            var isFailed = await sender.Send(new FailedPaymentCommand(paymentId.Value.ToString()));
            if(isFailed.IsFailed)
                throw new Exception($"Cannot proccess failed payment: {isFailed.Error.Code}");
        }

        await sender.Send(new ConfirmPaymentCommand(paymentId.Value.ToString()));
    }
}