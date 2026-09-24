using SharedKernel.Domain.Primivites.DomainEvent;

namespace SharedKernel.Domain.Abstractions;
public abstract class Entity<TId> where TId : notnull
{    
    public TId Id { get; protected set; } = default!;
    protected List<DomainEvent> domainEvents = new();

    public IReadOnlyList<DomainEvent> DomainEvents => domainEvents;

    public void Raise(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public void ClearDomainEvents() => domainEvents.Clear();

    protected Entity()
    {}
}