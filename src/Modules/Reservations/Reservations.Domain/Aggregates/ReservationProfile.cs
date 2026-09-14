using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Domain.Aggregates;
public class ReservationProfile : Entity<ReservationProfileId>
{
    public UserId UserId {get; private set;}
    private List<Reservation> _reservations {get; set;} = new ();
    public IReadOnlyList<Reservation> reservations => _reservations;

    private ReservationProfile(ReservationProfileId id, UserId userId) : base(id)
    {
        Id = id;
        UserId = userId;
    }

    public static ReservationProfile CreateProfile(UserId userId) =>
            new ReservationProfile(new ReservationProfileId(Guid.NewGuid()), userId);

    public void AddReservation(Guid gameId, DateTime startDate, DateTime endDate)
    {
        var reservation = new Reservation(new ReservationId(Guid.NewGuid()), gameId, startDate, endDate);
        _reservations.Add(reservation);
    }

    private ReservationProfile() {}
}