using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.DomainEvent;

namespace Identity.Domain.Abstractions;
public abstract class Entity : IdentityUser
{
    private List<DomainEvent> _domainEvents = new();
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    public void Raise(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}