using MassTransit;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.EventConsumers;
public class PaymentFailedEventConsumer(
ILogger<PaymentFailedEventConsumer> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : IConsumer<PaymentFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        logger.LogInformation("Consuming PaymentFailed event");

        var reservationId = new ReservationId(context.Message.ReservationId);

        var reservation = await reservationsRepository.GetReservationById(reservationId, context.CancellationToken);
        if(reservation is null)
        {
            throw new Exception($"Reservation with ID: {context.Message.ReservationId}, cannot be found");
        }
        reservation.Failed();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation($"Payment failed, event consumed");
    }
}