using GameReserve.IntegrationTests.Seeders;
using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Infrastructure;

public class TestDataSeeder
{
    private readonly IdentityContext _identityContext;
    private readonly GamesContext _gamesContext;

    private GamesTestDataSeeder _gamesTestDataSeeder;
    private IdentityTestDataSeeder _identityTestDataSeeder;
    
    public TestDataSeeder(IdentityContext identityContext, GamesContext gamesContext)
    {
        _identityContext = identityContext;
        _gamesContext = gamesContext;

        _gamesTestDataSeeder = new GamesTestDataSeeder(_gamesContext);
        _identityTestDataSeeder = new IdentityTestDataSeeder(_identityContext);
    }

    public async Task SeedAsync()
    {
        await _gamesTestDataSeeder.SeedGamesAsync();
        await _identityTestDataSeeder.SeedAsync();
    }
}