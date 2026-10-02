using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.Commands;
public record ConfirmPaymentCommand(string ExternalPaymentId) : ICommand<Result>;
public class ConfirmPaymentCommandHandler(
    IPaymentsRepository paymentsRepository, 
    ILogger<ConfirmPaymentCommandHandler> logger, 
    IPublisher publisher) : IRequestHandler<ConfirmPaymentCommand, Result>
{
    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new ExternalPaymentId(request.ExternalPaymentId), cancellationToken);

        if(payment is null)
        {
            logger.LogWarning($"Payment with external ID: {request.ExternalPaymentId} is not found in db");
            return PaymentErrors.PaymentWithExternalIdNotFound;
        }

        payment.Complete();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment confirmed");

        await publisher.Publish(new PaymentConfirmedIntegrationEvent(Guid.NewGuid(), payment.ReservationId.Value));
        return Result.Success;
    }
}