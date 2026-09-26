using MediatR;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Iterfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;

namespace Payments.Application.Commands;
public record CreateNewPaymentCommand(ReservationId ReservationId) : ICommand;
public class CreateNewPaymentCommandHandler(IPaymentGateway paymentGateway, IPaymentsRepository paymentsRepository) : IRequestHandler<CreateNewPaymentCommand>
{
    public async Task Handle(CreateNewPaymentCommand request, CancellationToken cancellationToken)
    {
        decimal amount = 40.00m;
        var externalPaymentId = await paymentGateway.CreatePaymentIntentAsync(request.ReservationId, amount);

        var payment = Payment.CreatePayment(request.ReservationId, externalPaymentId, amount);

        paymentsRepository.AddPayment(payment);
        await paymentsRepository.SaveChangesAsync(cancellationToken);
    }
}