using Games.Domain.Abstractions;

namespace Games.Domain.GameAggregate;
public class Game : Entity
{
    public string Title {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    
    private Game()
    {}
}