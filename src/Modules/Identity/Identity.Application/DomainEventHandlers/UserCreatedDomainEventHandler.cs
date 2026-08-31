using Identity.Domain.UserAggregate;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Identity.Application.DomainEventHandlers;
public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private IPublishEndpoint _publishEndpoint;
    private ILogger<UserCreatedDomainEventHandler> _logger;
    public UserCreatedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<UserCreatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserCreatedIntegrationEvent(Guid.NewGuid(), notification.UserId);

        try
        {
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation($"Succesfully send UserCreatedIntegrationEvent {notification.Id}", integrationEvent.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Cannot send event for user {notification.UserId}");            
        }
    }
}