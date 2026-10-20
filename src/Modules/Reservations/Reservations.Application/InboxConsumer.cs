using MassTransit;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Interfaces;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.InboxConsumer;
public class ReservationsInboxConsumer<TMessage>(IReservationsInboxRepository reservationsInbox, ILogger<ReservationsInboxConsumer<TMessage>> logger) : 
                                                IConsumer<TMessage> where TMessage : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        var messageId = context.Message.Id;
        var isDuplicate = await reservationsInbox.FindDuplicate(messageId);
        if(isDuplicate)
        {
            logger.LogWarning("This event is already in ReservationsInbox table", messageId);
            return;
        }
        await reservationsInbox.AddInboxMessage(context.Message, messageId, context.CancellationToken);
    }
}