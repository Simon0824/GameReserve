using SharedKernel.Domain.Abstractions;

namespace Reservations.Domain.Aggregates;
public class ReservationErrors
{
    public static readonly Error ReservationOverlapping = Error.Conflict("Reservation.Overlaps", "Your reservation overlaps with existing reservation");
    public static readonly Error ReservationProfileNotFoundByUserId = Error.NotFound("ReservationProfile.NotFoundByUser", "You don't have a reservation profile");
    public static readonly Error ReservationProfileNotFoundById = Error.NotFound("ReservationProfile.NotFoundById", "Reservation profile not found by requested ID");
    public static readonly Error ReservationStartDateGreaterOrEqual = Error.Validation("Reservation.EndBeforeStart", "Start date has to be set before end date");
    public static readonly Error ReservationPaymentAlreadyFailed = Error.Conflict("Reservation.AlreadyFailed", "Reservation payment has already failed");
    public static readonly Error GameIdNotFoundInDB = Error.NotFound("Game.NotFoundById", "Game with this ID does not exist");
}