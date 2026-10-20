using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Commands;
using Payments.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.NotificationHandlers;
public class ReservationCreatedEventHandler(ILogger<ReservationCreatedEventHandler> logger, ISender sender) : 
                                                                                INotificationHandler<ReservationCreatedIntegrationEvent>
{
    public async Task Handle(ReservationCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handling ReservationCreated notification with ID: {notification.ReservationId}");
        await sender.Send(new CreateNewPaymentCommand(new ReservationId(notification.ReservationId), notification.Amount));
    }
}