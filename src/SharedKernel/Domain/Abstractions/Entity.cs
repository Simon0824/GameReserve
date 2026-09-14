namespace SharedKernel.Domain.Abstractions;
public abstract class Entity<TId> where TId : notnull
{
    public TId Id {get;  set;} = default!;

    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity()
    {}
}