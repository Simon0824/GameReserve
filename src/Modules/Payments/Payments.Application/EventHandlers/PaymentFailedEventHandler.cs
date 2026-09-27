using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventHandlers;
public class PaymentFailedEventHandler : INotificationHandler<PaymentFailedIntegrationEvent>
{
    private IPublishEndpoint _publishEndpoint;
    private ILogger<PaymentFailedEventHandler> _logger;
    public PaymentFailedEventHandler(IPublishEndpoint publishEndpoint, ILogger<PaymentFailedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PaymenyFailed event");
        try
        {
            await _publishEndpoint.Publish(notification, cancellationToken);
            _logger.LogInformation($"Succesfully send PaymentFailedEvent for reservation {notification.ReservationId}", notification.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for event: {notification.Id}");      
        }
    }
}