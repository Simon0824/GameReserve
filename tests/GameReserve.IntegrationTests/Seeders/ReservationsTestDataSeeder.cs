using Bogus;
using Games.Domain.GameAggregate;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Primitives;
using Reservations.Infrastructure.Data;

namespace GameReserve.IntegrationTests.Seeders;

public class ReservationsTestDataSeeder
{
    private readonly ReservationsContext _reservationsContext;
    private readonly Faker _faker;
    private readonly List<Reservation> _reservations = new();
    private readonly List<ReservationProfile> _reservationProfiles = new();

    private readonly GamesTestDataSeeder _gamesSeeder;
    private readonly IdentityTestDataSeeder _identitySeeder;
    public ReservationsTestDataSeeder(ReservationsContext reservationsContext, 
                                      GamesTestDataSeeder gamesSeeder,
                                      IdentityTestDataSeeder identitySeeder)
    {
        _reservationsContext = reservationsContext;
        _gamesSeeder = gamesSeeder;
        _identitySeeder = identitySeeder;
        _faker = new Faker();
    }

    public async Task SeedAsync()
    {
        await SeedProfiles();
        await SeedReservations();

        await _reservationsContext.SaveChangesAsync();
    }

    public async Task SeedReservations()
    {
        var games = _gamesSeeder.Games;
        var currentCursor = DateTimeOffset.UtcNow.AddDays(-10);

        var reservationFaker = new Faker<Reservation>()
                                .CustomInstantiator(f => 
                                {
                                    var startDate = currentCursor.AddHours(f.Random.Int(1, 24));
                                    var endDate = startDate.AddHours(f.Random.Int(1, 4));

                                    var result = Reservation.CreateReservation(
                                                         new ReservationProfileId(f.PickRandom(_reservationProfiles).Id.Value),
                                                                                  f.PickRandom<Game>(games).Id.Value,
                                                                                  startDate,
                                                                                  endDate);
                                    if(result.IsFailed)
                                    {
                                        throw new Exception("Cannot create reservation");
                                    }

                                    return result.Value!;
                                });

        _reservations.AddRange(reservationFaker.Generate(10));

        await _reservationsContext.AddRangeAsync(_reservations);
    }

    public async Task SeedProfiles()
    {
        var users = _identitySeeder.Users;

        foreach(var user in users)
        {
            bool tryGuid = Guid.TryParse(user.Id, out Guid userId);

            var profile = ReservationProfile.CreateProfile(new UserId(userId));

            _reservationProfiles.Add(profile);
        }

        await _reservationsContext.AddRangeAsync(_reservationProfiles);
    }
}
