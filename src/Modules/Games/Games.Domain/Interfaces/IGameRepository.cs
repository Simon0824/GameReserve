using Games.Domain.GameAggregate;
using Games.Domain.Primitives;

namespace Games.Domain.Interfaces;
public interface IGameRepository
{
    Task AddGame(Game game);
    Task<Game?> FindGame(string Title);
    Task SaveChanges(CancellationToken cancellationToken);
}