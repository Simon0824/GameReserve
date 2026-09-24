using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Aggregates;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.DomainEventHandlers;
public class ReservationCreatedEventHandler : INotificationHandler<ReservationCreatedDomainEvent>
{
    private IPublishEndpoint _publishEndpoint;
    private ILogger<ReservationCreatedEventHandler> _logger;
    public ReservationCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<ReservationCreatedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task Handle(ReservationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ReservationCreatedIntegrationEvent(Guid.NewGuid(), notification.ReservationId.Value);

        try
        {
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation($"Succesfully send ReservationCreatedIntegrationEvent {notification.Id}", integrationEvent.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for reservation: {notification.ReservationId.Value}");            
        }
    }
}