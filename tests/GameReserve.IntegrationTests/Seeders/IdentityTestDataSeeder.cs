using Bogus;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;

namespace GameReserve.IntegrationTests;

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
        for(int user=0; user<=5; user++)
        {
            var tokenFaker = new Faker<RefreshToken>()
                                .CustomInstantiator(f => 
                                    RefreshToken.CreateRefreshToken(_users[user].Id, f.Random.AlphaNumeric(15), _users[user]));

            _refreshTokens.Add(tokenFaker.Generate());
        }

        await _identityContext.refreshTokens.AddRangeAsync(_refreshTokens);
    }
}
