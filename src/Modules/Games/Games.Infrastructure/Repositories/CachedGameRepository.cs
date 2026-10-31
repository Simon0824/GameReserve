using System.Text.Json;
using Games.Domain.GameAggregate;
using Games.Domain.Interfaces;
using Games.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Caching.Distributed;
using Scrutor;

namespace Games.Infrastructure.Repositories;
public class CachedGameRepository(IDistributedCache cache, IGameRepository decorated) : IGameRepository
{
    private const string _cachedGames = "games:all";
    private static string GamesByIdKey(string id) => $"game:{id}";
    private static string GamesByTitleKey(string title) => $"game:{title}";

    DistributedCacheEntryOptions GamesTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    DistributedCacheEntryOptions GamesByIdTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    DistributedCacheEntryOptions GamesByTitleTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public void AddGame(Game game)
    {
        decorated.AddGame(game);
        cache.Remove(_cachedGames);
    }

    public async Task DeleteGameAsync(GameId gameId, CancellationToken cancellationToken)
    {
        var game = await decorated.FindGameById(gameId,cancellationToken);
        await decorated.DeleteGameAsync(gameId, cancellationToken);
        await InvalidateGames(game!);
    }

    public async Task<Game?> FindGame(string title, CancellationToken cancellationToken)
    {
        var cachedGame = await cache.GetStringAsync(GamesByTitleKey(title), cancellationToken);

        Game? game;
        if(string.IsNullOrEmpty(cachedGame))
        {
            game = await decorated.FindGame(title, cancellationToken);

            if(game == null)
                return game;

            await cache.SetStringAsync(GamesByTitleKey(title), JsonSerializer.Serialize(game), GamesByTitleTtl, cancellationToken);

            return game;
        }

        var deserializedGame = JsonSerializer.Deserialize<Game?>(cachedGame);

        game = deserializedGame;

        return game;
    }

    public async Task<Game?> FindGameById(GameId gameId, CancellationToken cancellationToken)
    {
        var cachedGame = await cache.GetStringAsync(GamesByIdKey(gameId.ToString()), cancellationToken);

        Game? game;
        if(string.IsNullOrEmpty(cachedGame))
        {
            game = await decorated.FindGameById(gameId, cancellationToken);

            if(game == null)
                return game;

            await cache.SetStringAsync(GamesByIdKey(gameId.ToString()), JsonSerializer.Serialize(game), GamesByIdTtl, cancellationToken);

            return game;
        }

        var deserializedGame = JsonSerializer.Deserialize<Game?>(cachedGame);

        game = deserializedGame;

        return game;
    }

    public async Task<List<Game>> GetGames(CancellationToken cancellationToken)
    {
        var cachedGames = await cache.GetStringAsync(_cachedGames, cancellationToken);

        List<Game> games;
        if(string.IsNullOrEmpty(cachedGames))
        {
            games = await decorated.GetGames(cancellationToken);

            if(games == null)
                return games!;

            await cache.SetStringAsync(_cachedGames, JsonSerializer.Serialize(games), GamesTtl, cancellationToken);

            return games;
        }

        var deserializedGames = JsonSerializer.Deserialize<List<Game>>(cachedGames);

        games = deserializedGames!;

        return games;
    }


    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await decorated.SaveChanges(cancellationToken);
    }

    private async Task InvalidateGames(Game game)
    {
        await cache.RemoveAsync(_cachedGames);
        await cache.RemoveAsync(GamesByIdKey(game.Id.ToString()));
        await cache.RemoveAsync(GamesByTitleKey(game.Title));
    }
}