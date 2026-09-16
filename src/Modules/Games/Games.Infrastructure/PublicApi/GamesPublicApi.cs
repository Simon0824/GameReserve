using Games.Domain.Primitives;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.PublicApi.Games;

internal class GamePublicApi(GamesContext context) : IGamePublicApi
{

    public async Task<GameResultDTO?> GetGameById(Guid gameId, CancellationToken cancellationToken = default)
    {
        var id = new GameId(gameId);
        
        return await context.games
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GameResultDTO(g.Id.Value, g.Title))
            .FirstOrDefaultAsync(cancellationToken);
    }
}