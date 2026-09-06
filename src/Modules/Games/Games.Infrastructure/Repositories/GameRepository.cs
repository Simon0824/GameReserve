using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;
public class GameRepository(GamesContext context) : IGameRepository
{
    public async Task AddGame(Game game)
    {
        await context.games.AddAsync(game);
    }

    public async Task<Game?> FindGame(GameId gameId)
    {
        return await context.games
            .FirstOrDefaultAsync(g => g.GameId == gameId);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}