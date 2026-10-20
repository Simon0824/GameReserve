using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.EventConsumers;
public class PaymentConfirmedEventConsumer(
ILogger<PaymentConfirmedEventConsumer> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : INotificationHandler<PaymentConfirmedIntegrationEvent>
{
    public async Task Handle(PaymentConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling PaymentConfirmed event");

        var reservationId = new ReservationId(notification.ReservationId);

        var reservation = await reservationsRepository.GetReservationById(reservationId, cancellationToken);
        if(reservation is null)
        {
            throw new Exception($"Reservation with ID: {notification.ReservationId}, cannot be found");
        }
        reservation.Completed();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"Payment confirmed and event consumed succesfully!");
    }
}