using Games.Domain.GameAggregate;
using Games.Domain.Primitives;

namespace Games.Domain.Abstractions;
public abstract class Entity
{
    public GameId GameId {get; private set;}

    protected Entity(GameId gameId)
    {
        GameId = gameId;
    }

    protected Entity() => GameId = null!;
}