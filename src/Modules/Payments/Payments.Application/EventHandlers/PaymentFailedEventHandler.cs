using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventHandlers;
public class PaymentFailedEventHandler : INotificationHandler<PaymentFailedIntegrationEvent>
{
    private readonly IPaymentsOutboxRepository _paymentsOutboxRepository;
    private readonly ILogger<PaymentFailedEventHandler> _logger;
    public PaymentFailedEventHandler(IPaymentsOutboxRepository paymentsOutboxRepository, ILogger<PaymentFailedEventHandler> logger)
    {
        _paymentsOutboxRepository = paymentsOutboxRepository;
        _logger = logger;
    }    
    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PaymenyFailed event");
        try
        {
            _paymentsOutboxRepository.InsertOutboxMessage(notification);
            _logger.LogInformation($"Succesfully send PaymentFailedEvent for reservation {notification.ReservationId}", notification.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for event: {notification.Id}");      
        }
    }
}