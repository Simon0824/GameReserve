using MediatR;
using Payments.Application.Iterfaces;
using Payments.Domain.Aggregates;
using Payments.Domain.Enums;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using Payments.Infrastructure.Nbp;

namespace Payments.Application.Commands;
public record CreateNewPaymentCommand(ReservationId ReservationId, decimal Amount, PaymentCurrency PaymentCurrency, CardTypes CardType) : IRequest;
public class CreateNewPaymentCommandHandler(IPaymentGateway paymentGateway, IPaymentsRepository paymentsRepository, INbpRateClient nbp) : IRequestHandler<CreateNewPaymentCommand>
{
    public async Task Handle(CreateNewPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.CreatePayment(request.ReservationId, request.Amount);

        paymentsRepository.AddPayment(payment);
        await paymentsRepository.SaveChangesAsync(cancellationToken);

        var amount = await nbp.GetCurrencyExchange(PaymentCurrency.usd, request.Amount, cancellationToken);

        var externalPaymentId = await paymentGateway.CreatePaymentIntentAsync(request.ReservationId, payment.Id,amount, request.PaymentCurrency, request.CardType);

        var isSet = await paymentsRepository.SetExternalIdToPayment(payment.Id, new ExternalPaymentId(externalPaymentId), cancellationToken);

        if(!isSet)
            throw new Exception("Cannot set an external id to the payment in database");

        await paymentsRepository.SaveChangesAsync(cancellationToken);
    }
}