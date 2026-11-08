using System.Text.Json.Serialization;
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

    [JsonConstructor]
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

    public static Reservation Rehydrate(
        ReservationId id, 
        ReservationProfileId reservationProfileId, 
        Guid gameId, 
        DateTimeOffset startDate, 
        DateTimeOffset endDate,
        ReservationStatus status)
    {
        return new Reservation()
        {
            Id = id,
            ReservationProfileId = reservationProfileId,
            GameId = gameId,
            StartDate = startDate,
            EndDate = endDate,
            Status = status
        };
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

    public static decimal CalculatePrice(string gameCategory)
    {
        return gameCategory switch
        {
            "AAA" => 100.00m,
            "AA" => 70.00m,
            "III" => 50.00m,
            "Indie" => 20.00m,
            _ => 5.00m
        };
    }
    private Reservation()
    {}
}