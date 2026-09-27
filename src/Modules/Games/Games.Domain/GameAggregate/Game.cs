using SharedKernel.Domain.Abstractions;
using Games.Domain.Primitives;
using Games.Domain.Enums;

namespace Games.Domain.GameAggregate;
public class Game : Entity<GameId>
{
    public string Title {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    public GameCategory Category {get; private set;}
    
    private Game(GameId id, string title, string description, GameCategory category) : base(id)
    {
        Title = title;
        Description = description;
        Category = category;
    }

    public static Game Create(string title, string description, GameCategory gameCategory) => 
                                                new Game(new GameId(Guid.NewGuid()), title, description, gameCategory);

    private Game()
    {}
}