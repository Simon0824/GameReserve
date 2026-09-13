namespace Games.Domain.Primitives;
public readonly record struct GameId(Guid Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }
};