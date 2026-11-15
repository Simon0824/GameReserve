using GameReserve.IntegrationTests.Seeders;
using Games.Infrastructure.Data;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Reservations.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Infrastructure;

public class TestDataSeeder
{
    private readonly IdentityContext _identityContext;
    private readonly GamesContext _gamesContext;
    private readonly ReservationsContext _reservationsContext;
    private readonly UserManager<User> _userManager;
    private GamesTestDataSeeder _gamesTestDataSeeder;
    private IdentityTestDataSeeder _identityTestDataSeeder;
    private ReservationsTestDataSeeder _reservationsTestDataSeeder;
    public TestDataSeeder(UserManager<User> userManager, 
                            IdentityContext identityContext, 
                            GamesContext gamesContext, 
                            ReservationsContext reservationsContext)
    {
        _identityContext = identityContext;
        _gamesContext = gamesContext;
        _reservationsContext = reservationsContext;

        _userManager = userManager;

        _gamesTestDataSeeder = new GamesTestDataSeeder(_gamesContext);
        _identityTestDataSeeder = new IdentityTestDataSeeder(_identityContext, _userManager);
        _reservationsTestDataSeeder = new ReservationsTestDataSeeder(_reservationsContext, _gamesTestDataSeeder, _identityTestDataSeeder);
    }

    public async Task SeedAsync()
    {
        await _identityTestDataSeeder.SeedAsync();
        await _gamesTestDataSeeder.SeedGamesAsync();
        await _reservationsTestDataSeeder.SeedAsync();
    }
}