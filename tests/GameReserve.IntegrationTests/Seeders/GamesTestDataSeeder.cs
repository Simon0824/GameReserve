using Bogus;
using Games.Domain.Enums;
using Games.Domain.GameAggregate;
using Games.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Seeders;

public class GamesTestDataSeeder
{
    private readonly GamesContext _gamesContext;
    private readonly List<Game> _games = new();

    public GamesTestDataSeeder(GamesContext identityContext)
    {
        _gamesContext = identityContext;
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


        _games.AddRange(gameFaker.Generate(10));

        await _gamesContext.SaveChangesAsync();
    }
}
