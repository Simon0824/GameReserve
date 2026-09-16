using System.ComponentModel.DataAnnotations;
using Reservations.Domain.Primitives;

namespace Reservations.Domain.Aggregates;
public class Reservation
{
    public ReservationId Id {get; private set;} = new ReservationId(Guid.Empty);

    public ReservationProfileId ReservationProfileId {get; private set;}
    public Guid GameId {get; private set;}
    public DateTime StartDate {get; private set;}
    public DateTime EndDate {get; private set;}

    private Reservation(
        ReservationId id, 
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTime startDate, DateTime endDate)
    {
        if(startDate >= endDate)
        {
            throw new Exception("Start date has to be set before end date");
        }
        Id = id;
        ReservationProfileId = reservationProfileId;
        GameId = gameId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Reservation CreateReservation(
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTime startDate, DateTime endDate)
    {
        if(startDate > endDate)
        {
            throw new Exception("Start date cannot be after ending date");
        }
        
         var reservation = new Reservation(
         new ReservationId(Guid.NewGuid()),
          reservationProfileId,
          gameId,
          startDate,
          endDate);
        return reservation;
    }

    private Reservation()
    {}
}