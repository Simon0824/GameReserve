using Games.Domain.GameAggregate;
using Games.Domain.Primitives;

namespace Games.Domain.Interfaces;
public interface IGameRepository
{
    Task AddGame(Game game, CancellationToken cancellationToken);
    Task<Game?> FindGame(string Title, CancellationToken cancellationToken);
    Task<List<Game>> GetGames();
    Task SaveChanges(CancellationToken cancellationToken);
}