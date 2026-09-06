using Games.Domain.Abstractions;
using Games.Domain.Primitives;

namespace Games.Domain.GameAggregate;
public class Game : Entity
{
    public string Title {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    
    private Game(GameId id, string title, string description) : base(id)
    {
        Title = title;
        Description = description;
    }

    public static Game Create(string title, string description) => new Game(new GameId(Guid.NewGuid()), title, description);

    private Game()
    {}
}