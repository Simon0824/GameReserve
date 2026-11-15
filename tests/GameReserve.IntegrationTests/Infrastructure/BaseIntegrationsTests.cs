using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Infrastructure;
public class BaseIntegrationTests : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly IdentityContext _identityContext;
    private readonly ReservationsContext _reservationsContext;
    private readonly GamesContext _gamesContext;
    private readonly ISender _sender;
    private TestDataSeeder DataSeeder;

    public BaseIntegrationTests(IntegrationTestsWebFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        _identityContext = _scope.ServiceProvider.GetRequiredService<IdentityContext>();
        _reservationsContext = _scope.ServiceProvider.GetRequiredService<ReservationsContext>();
        _gamesContext = _scope.ServiceProvider.GetRequiredService<GamesContext>();

        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();

        DataSeeder = new TestDataSeeder(_identityContext, _gamesContext, _reservationsContext);
    }

    public Task InitializeAsync()
    {
        return DataSeeder.SeedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}