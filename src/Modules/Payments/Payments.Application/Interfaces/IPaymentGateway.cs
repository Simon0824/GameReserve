using Payments.Domain.Enums;
using Payments.Domain.Primitives;

namespace Payments.Application.Iterfaces;
public interface IPaymentGateway
{
    Task<string> CreatePaymentIntentAsync(ReservationId reservationId,
                                          PaymentId paymentId, 
                                          decimal amount, 
                                          PaymentCurrency currency, 
                                          CardTypes cardType);
}