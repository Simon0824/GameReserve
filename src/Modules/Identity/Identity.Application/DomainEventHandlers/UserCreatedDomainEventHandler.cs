using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Identity.Application.DomainEventHandlers;
public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IIdentityOutboxRepository _identityOutboxRepository;
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(
                    IIdentityOutboxRepository identityOutboxRepository, 
                    ILogger<UserCreatedDomainEventHandler> logger)
    {
        _identityOutboxRepository = identityOutboxRepository;
        _logger = logger;
    }
    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserCreatedIntegrationEvent(Guid.NewGuid(), notification.UserId);

        try
        {
            _identityOutboxRepository.InsertOutboxMessage(integrationEvent);
            _logger.LogInformation($"Succesfully send UserCreatedIntegrationEvent {notification.Id}, ", integrationEvent.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Cannot send UserCreatedIntegrationEvent");
            throw;
        }
        
    }
}