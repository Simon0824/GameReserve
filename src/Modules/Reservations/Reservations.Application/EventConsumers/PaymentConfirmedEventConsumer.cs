using MassTransit;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.EventConsumers;
public class PaymentConfirmedEventConsumer(
ILogger<PaymentConfirmedEventConsumer> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : IConsumer<PaymentConfirmedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentConfirmedIntegrationEvent> context)
    {
        logger.LogInformation("Consuming PaymentConfirmed event");

        var reservationId = new ReservationId(context.Message.ReservationId);

        var reservation = await reservationsRepository.GetReservationById(reservationId, context.CancellationToken);
        if(reservation is null)
        {
            throw new Exception($"Reservation with ID: {context.Message.ReservationId}, cannot be found");
        }
        reservation.Completed();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation($"Payment confirmed and event consumed succesfully!");
    }
}