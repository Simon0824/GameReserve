using SharedKernel.Domain.Abstractions;

namespace Games.Domain.GameAggregate;
public class GamesErrors
{
    public static readonly Error GameNotFound = Error.NotFound("Games.NotFOundById", "Game with this ID is not found in DB");
    public static readonly Error GameAlreadyExist = Error.Conflict("Games.GamesAlreadyInDB", "This game is already in DB");
}