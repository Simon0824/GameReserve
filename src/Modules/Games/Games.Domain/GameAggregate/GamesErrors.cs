using SharedKernel.Domain.Abstractions;

namespace Games.Domain.GameAggregate;
public class GamesErrors
{
    public static readonly Error GameNotFound = new("Games.NotFOundById", "Game with this ID is not found in DB");
    public static readonly Error GameAlreadyExist = new("Games.GamesAlreadyInDB", "This game is already in DB");
}