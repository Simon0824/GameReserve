using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.PublicApi.Games;

internal class GamePublicApi(GamesContext context) : IGamePublicApi
{

    public async Task<GameDto?> GetGameById(Guid gameId, CancellationToken cancellationToken = default)
    {
        return await context.games
            .AsNoTracking()
            .Where(g => g.Id.Value == gameId)
            .Select(g => new GameDto(g.Id.Value, g.Title))
            .FirstOrDefaultAsync(cancellationToken);
    }
}