using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Domain.Aggregates;
public class ReservationProfile : Entity<ReservationProfileId>
{
    public UserId UserId {get; private set;}

    private readonly List<Reservation> _reservations = new();
    public IReadOnlyList<Reservation> Reservations => _reservations;

    private ReservationProfile(ReservationProfileId id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    public static ReservationProfile CreateProfile(UserId userId) =>
            new ReservationProfile(new ReservationProfileId(Guid.NewGuid()), userId);

    public static Reservation AddReservation(Guid gameId, DateTime startDate, DateTime endDate)
    {
        var reservation = Reservation.CreateReservation(Id, gameId, startDate, endDate);
        _reservations.Add(reservation);
        return reservation;
    }
    private ReservationProfile() {}
}