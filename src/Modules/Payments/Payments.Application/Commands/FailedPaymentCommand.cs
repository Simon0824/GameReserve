using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Aggregates;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.Commands;
public record FailedPaymentCommand(string ExternalPaymentId) : ICommand<Result>;
public class FailedPaymentCommandHandler(
    IPaymentsRepository paymentsRepository, 
    ILogger<FailedPaymentCommandHandler> logger, 
    IPublisher publisher) : IRequestHandler<FailedPaymentCommand, Result>
{
    public async Task<Result> Handle(FailedPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new ExternalPaymentId(request.ExternalPaymentId), cancellationToken);

        if(payment is null) 
        {
            logger.LogWarning($"Payment with external ID: {request.ExternalPaymentId} is not found in db");
            return PaymentErrors.PaymentWithExternalIdNotFound;
        }

        payment.Failed();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment failed");

        await publisher.Publish(new PaymentFailedIntegrationEvent(Guid.NewGuid(), payment.ReservationId.Value));
        return Result.Success;
    }
}