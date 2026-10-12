using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventHandlers;
public class PaymentFailedEventHandler : INotificationHandler<PaymentFailedIntegrationEvent>
{
    private ILogger<PaymentFailedEventHandler> _logger;
    public PaymentFailedEventHandler(ILogger<PaymentFailedEventHandler> logger)
    {
        _logger = logger;
    }    
    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PaymenyFailed event");
        try
        {
            _logger.LogInformation($"Succesfully send PaymentFailedEvent for reservation {notification.ReservationId}", notification.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for event: {notification.Id}");      
        }
    }
}