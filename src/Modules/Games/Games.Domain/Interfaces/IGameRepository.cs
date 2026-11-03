using Games.Domain.GameAggregate;
using Games.Domain.Primitives;

namespace Games.Domain.Interfaces;
public interface IGameRepository
{
    void AddGame(Game game);
    Task DeleteGameAsync(GameId gameId, CancellationToken cancellationToken);
    Task<Game?> FindGame(string Title, CancellationToken cancellationToken);
    Task<Game?> FindGameById(GameId gameId, CancellationToken cancellationToken);
    Task<List<Game>> GetGames(CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
}