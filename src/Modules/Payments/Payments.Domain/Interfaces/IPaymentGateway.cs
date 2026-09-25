using Payments.Domain.Primitives;

namespace Payments.Domain.Iterfaces;
public interface IPaymentGateway
{
    Task<string> CreatePaymentIntentAsync(ReservationId reservationId, decimal Amount);
}