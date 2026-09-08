using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;
public class GameRepository(GamesContext context) : IGameRepository
{
    public async Task AddGame(Game game, CancellationToken cancellationToken)
    {
        await context.games.AddAsync(game, cancellationToken);
    }

    public async Task<Game?> FindGame(string Title, CancellationToken cancellationToken)
    {
        return await context.games
            .FirstOrDefaultAsync(
                g => g.Title == Title,
                cancellationToken);
    }

    public async Task<Game?> FindGameById(GameId gameId, CancellationToken cancellationToken)
    {
        return await context.games
            .FirstOrDefaultAsync(
                g => g.GameId == gameId,
                cancellationToken);
    }

    public async Task<List<Game>> GetGames()
    {
        return await context.games.AsNoTracking().ToListAsync();
    }


    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}