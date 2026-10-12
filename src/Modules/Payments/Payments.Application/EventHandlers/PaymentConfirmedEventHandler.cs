using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventHandlers;
public class PaymentConfirmedEventHandler : INotificationHandler<PaymentConfirmedIntegrationEvent>
{
    private ILogger<PaymentConfirmedEventHandler> _logger;
    public PaymentConfirmedEventHandler(ILogger<PaymentConfirmedEventHandler> logger)
    {
        _logger = logger;
    }    
    public async Task Handle(PaymentConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PaymenyConfirmed event");
        try
        {
            _logger.LogInformation($"Succesfully send PaymentConfirmedEvent for reservation {notification.ReservationId}", notification.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for event: {notification.Id}");      
        }
    }
}