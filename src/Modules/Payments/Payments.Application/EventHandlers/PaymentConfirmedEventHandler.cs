using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventHandlers;
public class PaymentConfirmedEventHandler : INotificationHandler<PaymentConfirmedIntegrationEvent>
{
    private IPublishEndpoint _publishEndpoint;
    private ILogger<PaymentConfirmedEventHandler> _logger;
    public PaymentConfirmedEventHandler(IPublishEndpoint publishEndpoint, ILogger<PaymentConfirmedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task Handle(PaymentConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PaymenyConfirmed event");
        try
        {
            await _publishEndpoint.Publish(notification, cancellationToken);
            _logger.LogInformation($"Succesfully send PaymentConfirmedEvent for reservation {notification.ReservationId}", notification.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for event: {notification.Id}");      
        }
    }
}