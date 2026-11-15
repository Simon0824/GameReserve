using Games.Infrastructure.Data;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Infrastructure;
public class BaseIntegrationTests : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly IdentityContext IdentityContext;
    protected readonly ReservationsContext ReservationsContext;
    protected readonly GamesContext GamesContext;
    protected readonly ISender Sender;
    protected readonly UserManager<User> UserManager;
    private TestDataSeeder DataSeeder;

    public BaseIntegrationTests(IntegrationTestsWebFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        IdentityContext = _scope.ServiceProvider.GetRequiredService<IdentityContext>();
        ReservationsContext = _scope.ServiceProvider.GetRequiredService<ReservationsContext>();
        GamesContext = _scope.ServiceProvider.GetRequiredService<GamesContext>();
        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

        DataSeeder = new TestDataSeeder(UserManager, IdentityContext, GamesContext, ReservationsContext);
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