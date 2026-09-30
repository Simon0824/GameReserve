using SharedKernel.Domain.Abstractions;

namespace Reservations.Domain.Aggregates;
public class ReservationErrors
{
    public static readonly Error ReservationOverlapping = new("Reservation.Overlaps", "Your reservation overlaps with existing reservation");
    public static readonly Error ReservationProfileNotFoundByUserId = new("ReservationProfile.NotFoundByUser", "You don't have a reservation profile");
    public static readonly Error ReservationProfileNotFoundById = new("ReservationProfile.NotFoundById", "Reservation profile not found by requested ID");
    public static readonly Error ReservationStartDateGreaterOrEqual = new("Reservation.EndBeforeStart", "Start date has to be set before end date");
    public static readonly Error ReservationPaymentAlreadyFailed = new("Reservation.AlreadyFailed", "Reservation payment has already failed");
    public static readonly Error GameIdNotFoundInDB = new("Game.NotFoundById", "Game with this ID does not exist");
}