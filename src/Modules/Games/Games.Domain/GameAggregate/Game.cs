namespace Games.Domain.GameAggregate;
public class Game
{
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    
    private Game()
    {}
}