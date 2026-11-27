using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.Commands;
public record FailedPaymentCommand(string PaymentId) : ICommand<Result>;
public class FailedPaymentCommandHandler(
    IPaymentsRepository paymentsRepository, 
    ILogger<FailedPaymentCommandHandler> logger, 
    IPaymentsOutboxRepository paymentsOutbox) : IRequestHandler<FailedPaymentCommand, Result>
{
    public async Task<Result> Handle(FailedPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new PaymentId(Guid.Parse(request.PaymentId)), cancellationToken);

        if(payment is null) 
        {
            logger.LogWarning($"Payment with ID: {request.PaymentId} is not found in db");
            return PaymentErrors.PaymentWithExternalIdNotFound;
        }

        payment.Failed();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment failed");

        paymentsOutbox.InsertOutboxMessage(new PaymentFailedIntegrationEvent(payment.ReservationId.Value));
        return Result.Success;
    }
}