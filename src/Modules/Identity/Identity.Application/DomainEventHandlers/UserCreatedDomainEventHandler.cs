using Identity.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.DomainEventHandlers;
public class UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger) : INotificationHandler<UserCreatedDomainEvent>
{
    
}