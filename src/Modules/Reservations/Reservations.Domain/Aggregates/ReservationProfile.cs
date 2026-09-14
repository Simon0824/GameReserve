using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Domain.Aggregates;
public class ReservationProfile : Entity<ReservationProfileId>
{
    public Guid UserId {get; private set;}
    private List<Reservation> _reservations {get; set;} = new ();
    public IReadOnlyList<Reservation> reservations => _reservations;

    private ReservationProfile() {}
}