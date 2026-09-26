using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces;
using Payments.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;

namespace Payments.Application.Commands;
public record ConfirmPaymentCommand(string ExternalPaymentId) : ICommand;
public class ConfirmPaymentCommandHandler(IPaymentsRepository paymentsRepository, ILogger<ConfirmPaymentCommandHandler> logger) : IRequestHandler<ConfirmPaymentCommand>
{
    public async Task Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentsRepository.GetPaymentByExternalId(new ExternalPaymentId(request.ExternalPaymentId), cancellationToken);

        if(payment is null) throw new Exception($"Payment with external id: {request.ExternalPaymentId}, is not found");

        payment.Complete();
        await paymentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payment confirmed");
    }
}