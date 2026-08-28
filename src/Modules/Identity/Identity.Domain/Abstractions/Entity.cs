using Identity.Domain.Primitives;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Abstractions;
public abstract class Entity : IdentityUser
{
    private List<DomainEvent> _domainEvents = new();
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;
}