using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Iterfaces;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using Payments.Infrastructure.Nbp;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Enums;

namespace Payments.Application.Commands;
public record CreateNewPaymentCommand(
    ReservationId ReservationId, 
    decimal Amount, 
    decimal CalculatedPrice, 
    PaymentCurrency PaymentCurrency, 
    CardTypes CardType) : ICommand;
public class CreateNewPaymentCommandHandler(
    IPaymentGateway paymentGateway, 
    IPaymentsRepository paymentsRepository, 
    INbpRateClient nbp,
    ISender sender,
    ILogger<CreateNewPaymentCommandHandler> logger) : IRequestHandler<CreateNewPaymentCommand>
{
    public async Task Handle(CreateNewPaymentCommand request, CancellationToken cancellationToken)
    {
        var isDuplicate = await paymentsRepository.FindReservationDuplicate(request.ReservationId, cancellationToken);

        if(isDuplicate)
            throw new Exception($"There is a duplicate of this reservatio, ID: {request.ReservationId.Value}");

        var payment = Payment.CreatePayment(request.ReservationId, request.Amount);

        paymentsRepository.AddPayment(payment);
        await paymentsRepository.SaveChangesAsync(cancellationToken);

        var amount = await nbp.GetCurrencyExchange(request.PaymentCurrency, request.Amount, cancellationToken);

        if(request.PaymentCurrency == PaymentCurrency.none)
        {
            logger.LogWarning($"Currency is not set");  

            await sender.Send(new FailedPaymentCommand(payment.Id.Value.ToString()));
            return;
        }
        
        if(amount < request.CalculatedPrice)
        {
            var change = Math.Abs(amount - request.CalculatedPrice);

            logger.LogWarning($"Amount is less than actual price! You need {change}zl");

            await sender.Send(new FailedPaymentCommand(payment.Id.Value.ToString()));    

            return;
        }
        
        if(amount > request.CalculatedPrice)
        {
            var change = amount - request.CalculatedPrice;

            logger.LogWarning($"Amount is greater than actual price! We cannot give you a change, which is {change}zl");        
        }



        var externalPaymentId = await paymentGateway.CreatePaymentIntentAsync(
                                                                    request.ReservationId, 
                                                                    payment.Id,
                                                                    amount, 
                                                                    request.CalculatedPrice,
                                                                    request.PaymentCurrency, 
                                                                    request.CardType);

        var isSet = await paymentsRepository.SetExternalIdToPayment(payment.Id, new ExternalPaymentId(externalPaymentId), cancellationToken);

        if(!isSet)
            throw new Exception("Cannot set an external id to the payment in database");

        await paymentsRepository.SaveChangesAsync(cancellationToken);
    }
}