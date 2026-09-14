using Reservations.Domain.Primitives;

namespace Reservations.Domain.Aggregates;
public class Reservation
{
    public ReservationId Id {get; private set;} = new ReservationId(Guid.Empty);
    public Guid GameId {get; private set;}
    public DateTime StartDate {get; private set;}
    public DateTime EndDate {get; private set;}

    public Reservation(ReservationId id, Guid gameId, DateTime startDate, DateTime endDate)
    {
        if(startDate >= endDate)
        {
            throw new Exception("Start date has to be set before end date");
        }
        Id = id;
        GameId = gameId;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Reservation()
    {}
}