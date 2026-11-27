using Payments.Domain.Primitives;
using SharedKernel.Domain.Enums;

namespace Payments.Application.Iterfaces;
public interface IPaymentGateway
{
    Task<string> CreatePaymentIntentAsync(ReservationId reservationId,
                                          PaymentId paymentId, 
                                          decimal amount, 
                                          PaymentCurrency currency, 
                                          CardTypes cardType);
}