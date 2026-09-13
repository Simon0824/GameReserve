namespace Reservations.Domain.Aggregates;
public class Reservation
{
    public Guid Id {get; private set;}
    public Guid GameId {get; private set;}
    public DateTime StartDate {get; private set;}
    public DateTime EndDate {get; private set;}

    public Reservation(Guid id, Guid gameId, DateTime startDate, DateTime endDate)
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