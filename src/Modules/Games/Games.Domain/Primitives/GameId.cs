namespace Games.Domain.Primitives;
public readonly record struct GameId(Guid Id)
{
    public override string ToString()
    {
        return Id.ToString();
    }
};