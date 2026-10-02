using System.ComponentModel.DataAnnotations;
using Reservations.Domain.Enums;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

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
        DateTimeOffset startDate, 
        DateTimeOffset endDate)
    {
        Id = id;
        ReservationProfileId = reservationProfileId;
        GameId = gameId;
        StartDate = startDate;
        EndDate = endDate;
        Status = ReservationStatus.Pending;
    }

    public static Result<Reservation> CreateReservation(
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if(startDate >= endDate)
        {
            return ReservationErrors.ReservationStartDateGreaterOrEqual;
        }
        
         var reservation = new Reservation(
         new ReservationId(Guid.NewGuid()),
          reservationProfileId,
          gameId,
          startDate,
          endDate);
        return reservation;
    }

    public Result Completed()
    {
        if(Status == ReservationStatus.Completed)
        {
            return Result.Success;
        }
        else if(Status == ReservationStatus.Failed)
        {
            return Result.Failure(ReservationErrors.ReservationPaymentAlreadyFailed);
        }
        Status = ReservationStatus.Completed;

        return Result.Success;
    }

    public void Failed()
    {
        Status = ReservationStatus.Failed;
    }
    private Reservation()
    {}
}