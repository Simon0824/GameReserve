using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.Commands;
public record ConfirmPaymentCommand(string PaymentId) : ICommand<Result>;
public class ConfirmPaymentCommandHandler(
    IPaymentsRepository paymentsRepository, 
    ILogger<ConfirmPaymentCommandHandler> logger, 
    IPaymentsOutboxRepository paymentsOutbox) : IRequestHandler<ConfirmPaymentCommand, Result>
{
    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new PaymentId(Guid.Parse(request.PaymentId)), cancellationToken);

        if(payment is null)
        {
            logger.LogWarning($"Payment with ID: {request.PaymentId} is not found in db");
            return PaymentErrors.PaymentWithExternalIdNotFound;
        }

        payment.Complete();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment confirmed");

        paymentsOutbox.InsertOutboxMessage(new PaymentConfirmedIntegrationEvent(payment.ReservationId.Value));
        return Result.Success;
    }
}