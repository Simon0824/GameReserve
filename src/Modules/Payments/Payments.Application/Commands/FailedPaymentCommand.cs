using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;

namespace Payments.Application.Commands;
public record FailedPaymentCommand(string ExternalPaymentId) : ICommand;
public class FailedPaymentCommandHandler(IPaymentsRepository paymentsRepository, ILogger<FailedPaymentCommandHandler> logger) : IRequestHandler<FailedPaymentCommand>
{
    public async Task Handle(FailedPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new ExternalPaymentId(request.ExternalPaymentId), cancellationToken);

        if(payment is null) throw new Exception($"Payment with external id: {request.ExternalPaymentId}, is not found");

        payment.Failed();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment failed");
    }
}