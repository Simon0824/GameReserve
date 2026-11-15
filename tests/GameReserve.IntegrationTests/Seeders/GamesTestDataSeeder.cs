using Bogus;
using Games.Domain.Enums;
using Games.Domain.GameAggregate;
using Games.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Seeders;

public class GamesTestDataSeeder
{
    private readonly GamesContext _gamesContext;
    private readonly List<Game> _games = new();

    public IReadOnlyList<Game> Games => _games;

    public GamesTestDataSeeder(GamesContext? gamesContext = null)
    {
        _gamesContext = gamesContext!;
    }

    public async Task SeedGamesAsync()
    {
        if(_games.Any())
        {
            return;
        }

        var gameFaker = new Faker<Game>()
                                .CustomInstantiator(f => 
                                    Game.Create(
                                    f.Commerce.ProductName(), 
                                    f.Lorem.Sentence(20), 
                                    f.PickRandom<GameCategory>()));

        if(_gamesContext is not null)
        {
            _games.AddRange(gameFaker.Generate(10));
            
            await _gamesContext.SaveChangesAsync();
        }
    }

    public Game GetGame(int index = 0) => _games[index];
}
