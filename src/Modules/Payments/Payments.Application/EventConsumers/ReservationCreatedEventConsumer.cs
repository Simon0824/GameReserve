using MassTransit;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventConsumers;
public class ReservationCreatedEventConsumer(ILogger<ReservationCreatedEventConsumer> logger) : IConsumer<ReservationCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ReservationCreatedIntegrationEvent> context)
    {
        logger.LogInformation($"Consuming ReservationCreated event with ID: {context.Message.ReservationId}");
    }
}