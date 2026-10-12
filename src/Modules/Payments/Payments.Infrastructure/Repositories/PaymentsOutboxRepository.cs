using System.Text.Json;
using Payments.Domain.Interfaces;
using Payments.Infrastructure.Data;
using SharedKernel.Domain.Abstractions;

namespace Identity.Infrastructure.Repositories;
public class PaymentsOutboxRepository(PaymentsContext context) : IPaymentsOutboxRepository
{
    public void InsertOutboxMessage<T>(T message) where T : notnull
    {
        string JsonString = JsonSerializer.Serialize(message);

        JsonDocument jsonDocument = JsonDocument.Parse(JsonString);

        var outboxMessage =  new OutboxMessage()
        {
            Id = Guid.CreateVersion7(),
            Type = message.GetType().AssemblyQualifiedName!,
            Content = jsonDocument,
            OccuredOnTime = DateTime.UtcNow
        };

        context.OutboxMessages.Add(outboxMessage);
    }
}