using GameReserve.IntegrationTests.Seeders;
using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;
using Reservations.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Infrastructure;

public class TestDataSeeder
{
    private readonly IdentityContext _identityContext;
    private readonly GamesContext _gamesContext;
    private readonly ReservationsContext _reservationsContext;
    private GamesTestDataSeeder _gamesTestDataSeeder;
    private IdentityTestDataSeeder _identityTestDataSeeder;
    private ReservationsTestDataSeeder _reservationsTestDataSeeder;
    public TestDataSeeder(IdentityContext identityContext, GamesContext gamesContext, ReservationsContext reservationsContext)
    {
        _identityContext = identityContext;
        _gamesContext = gamesContext;
        _reservationsContext = reservationsContext;

        _gamesTestDataSeeder = new GamesTestDataSeeder(_gamesContext);
        _identityTestDataSeeder = new IdentityTestDataSeeder(_identityContext);
        _reservationsTestDataSeeder = new ReservationsTestDataSeeder(_reservationsContext, _gamesTestDataSeeder, _identityTestDataSeeder);
    }

    public async Task SeedAsync()
    {
        await _identityTestDataSeeder.SeedAsync();
        await _gamesTestDataSeeder.SeedGamesAsync();
        await _reservationsTestDataSeeder.SeedAsync();
    }
}