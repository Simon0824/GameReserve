using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Commands;
using Payments.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.EventConsumers;
public class ReservationCreatedEventConsumer(ILogger<ReservationCreatedEventConsumer> logger, ISender sender) : IConsumer<ReservationCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ReservationCreatedIntegrationEvent> context)
    {
        logger.LogInformation($"Consuming ReservationCreated event with ID: {context.Message.ReservationId}");
        await sender.Send(new CreateNewPaymentCommand(new ReservationId(context.Message.ReservationId)));
    }
}