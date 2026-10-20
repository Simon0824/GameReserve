using MediatR;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.EventHandlers;
public class PaymentFailedEventHandler(
ILogger<PaymentFailedEventHandler> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : INotificationHandler<PaymentFailedIntegrationEvent>
{
    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consuming PaymentFailed event");

        var reservationId = new ReservationId(notification.ReservationId);

        var reservation = await reservationsRepository.GetReservationById(reservationId, cancellationToken);
        if(reservation is null)
        {
            throw new Exception($"Reservation with ID: {notification.ReservationId}, cannot be found");
        }
        reservation.Failed();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"Payment failed, event consumed");
    }
}