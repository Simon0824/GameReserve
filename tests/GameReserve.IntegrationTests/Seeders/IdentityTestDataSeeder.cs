using Bogus;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Seeders;

public class IdentityTestDataSeeder
{
    private readonly IdentityContext _identityContext;
    private readonly Faker _faker;
    private readonly List<User> _users = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    public IdentityTestDataSeeder(IdentityContext identityContext)
    {
        _identityContext = identityContext;
        _faker = new Faker();
    }

    public async Task SeedAsync()
    {
        await SeedUsers();
        await SeedTokens();

        await _identityContext.SaveChangesAsync();
    }

    public async Task SeedUsers()
    {
        var userFaker = new Faker<User>()
                                .CustomInstantiator(f => User.Create(f.Person.FullName, f.Person.Email));

        _users.AddRange(userFaker.Generate(10));

        await _identityContext.AddRangeAsync(_users);
    }

    public async Task SeedTokens()
    {
        foreach(var user in _users.Take(5))
        {
            var refreshToken = RefreshToken.CreateRefreshToken(user.Id, _faker.Random.AlphaNumeric(10), user);

            _refreshTokens.Add(refreshToken);
        }

        await _identityContext.refreshTokens.AddRangeAsync(_refreshTokens);
    }
}
