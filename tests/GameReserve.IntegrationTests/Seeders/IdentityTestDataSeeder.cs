using Bogus;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace GameReserve.IntegrationTests.Seeders;

public class IdentityTestDataSeeder
{
    private readonly IdentityContext _identityContext;
    private readonly UserManager<User> _userManager;
    private readonly Faker _faker;
    private readonly List<User> _users = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyList<User> Users => _users;

    public IdentityTestDataSeeder(IdentityContext? identityContext, UserManager<User>? userManager)
    {
        _identityContext = identityContext!;
        _userManager = userManager!;
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
        if(_users.Any())
        {
            return;
        }

        var userFaker = new Faker<User>()
                                .CustomInstantiator(f => User.Create(f.Person.FullName, f.Person.Email));


        if(_identityContext is not null)
        {
            _users.AddRange(userFaker.Generate(10));

            foreach(var user in _users)
            {
                var isSucceded = await _userManager.CreateAsync(user, _faker.Internet.Password(10) + "!Ds0");

                if(!isSucceded.Succeeded)
                {
                    throw new Exception(string.Join(" ", isSucceded.Errors.Select(e => e.Description)));
                }
            }
        }
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
