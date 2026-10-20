using MassTransit;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces;
using SharedKernel.IntegrationEvents;

namespace Payments.Application.PaymentsInboxConsumer;
public class PaymentsInboxConsumer<TMessage>(
            ILogger<PaymentsInboxConsumer<TMessage>> logger,
            IPaymentsInboxRepository inboxRepository) : IConsumer<TMessage> where TMessage : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        var messageId = context.Message.Id;
        var isDuplicate = await inboxRepository.FindDuplicate(messageId);

        if(isDuplicate)
        {
            logger.LogWarning("This event is already in PaymentsInbox table", messageId);
            return;
        }

        await inboxRepository.AddInboxMessage(context.Message, messageId, context.CancellationToken);
    }
}