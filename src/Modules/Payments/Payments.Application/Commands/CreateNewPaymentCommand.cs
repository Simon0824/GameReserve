using MediatR;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Iterfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;

namespace Payments.Application.Commands;
public record CreateNewPaymentCommand(ReservationId ReservationId, decimal Amount) : ICommand;
public class CreateNewPaymentCommandHandler(IPaymentGateway paymentGateway, IPaymentsRepository paymentsRepository) : IRequestHandler<CreateNewPaymentCommand>
{
    public async Task Handle(CreateNewPaymentCommand request, CancellationToken cancellationToken)
    {
        var externalPaymentId = await paymentGateway.CreatePaymentIntentAsync(request.ReservationId, request.Amount);

        var payment = Payment.CreatePayment(request.ReservationId, externalPaymentId, request.Amount);

        paymentsRepository.AddPayment(payment);
        await paymentsRepository.SaveChangesAsync(cancellationToken);
    }
}