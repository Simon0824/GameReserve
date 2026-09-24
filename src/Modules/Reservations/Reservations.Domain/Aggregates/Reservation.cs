using System.ComponentModel.DataAnnotations;
using Reservations.Domain.Enums;
using Reservations.Domain.Primitives;

namespace Reservations.Domain.Aggregates;
public class Reservation
{
    public ReservationId Id {get; private set;} = new ReservationId(Guid.Empty);

    public ReservationProfileId ReservationProfileId {get; private set;}
    public Guid GameId {get; private set;}
    public DateTimeOffset StartDate {get; private set;}
    public DateTimeOffset EndDate {get; private set;}
    public ReservationStatus Status {get; private set;}

    private Reservation(
        ReservationId id, 
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTimeOffset startDate, DateTimeOffset endDate)
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
        Status = ReservationStatus.Pending;
    }

    public static Reservation CreateReservation(
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTimeOffset startDate, DateTimeOffset endDate)
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

    public void Completed()
    {
        if(Status == ReservationStatus.Completed)
        {
            return;
        }
        else if(Status == ReservationStatus.Failed)
        {
            throw new Exception("Reservation payment has already failed");
        }
        Status = ReservationStatus.Completed;
    }

    public void Failed()
    {
        Status = ReservationStatus.Failed;
    }
    private Reservation()
    {}
}