using System.Reflection.Metadata.Ecma335;
using Games.Domain.Abstractions;
using Games.Domain.Primitives;

namespace Games.Domain.GameAggregate;
public class Game : Entity
{
    public string Title {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    
    private Game(GameId id, string title) : base(id)
    {
        Title = title;
    }

    public static Game Create(string title) => new Game(new GameId(Guid.NewGuid()), title);

    private Game()
    {}
}