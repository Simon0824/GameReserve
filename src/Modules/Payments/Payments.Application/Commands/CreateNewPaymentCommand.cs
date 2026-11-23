using MediatR;
using Payments.Domain.Aggregates;
using Payments.Domain.Enums;
using Payments.Domain.Interfaces;
using Payments.Domain.Iterfaces;
using Payments.Domain.Primitives;
using Payments.Infrastructure.Nbp;
using SharedKernel.Application.Abstractions.Messaging;

namespace Payments.Application.Commands;
public record CreateNewPaymentCommand(ReservationId ReservationId, decimal Amount, PaymentCurrency PaymentCurrency, string CardType) : ICommand;
public class CreateNewPaymentCommandHandler(IPaymentGateway paymentGateway, IPaymentsRepository paymentsRepository, INbpRateClient nbp) : IRequestHandler<CreateNewPaymentCommand>
{
    public async Task Handle(CreateNewPaymentCommand request, CancellationToken cancellationToken)
    {
        var amount = await nbp.GetCurrencyExchange(PaymentCurrency.usd, request.Amount, cancellationToken);

        var externalPaymentId = await paymentGateway.CreatePaymentIntentAsync(request.ReservationId, amount, request.PaymentCurrency, request.CardType);

        var payment = Payment.CreatePayment(request.ReservationId, externalPaymentId, request.Amount);

        paymentsRepository.AddPayment(payment);
        await paymentsRepository.SaveChangesAsync(cancellationToken);
    }
}