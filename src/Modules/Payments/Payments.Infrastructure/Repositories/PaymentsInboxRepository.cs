using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Interfaces;
using Payments.Infrastructure.Data;
using SharedKernel.Domain.Abstractions;

namespace Payments.Infrastructure.Repositories;
public class PaymentsInboxRepository(PaymentsContext context) : IPaymentsInboxRepository
{
    public async Task AddInboxMessage<T>(T message, Guid messageId, CancellationToken cancellationToken) where T : notnull
    {
        var serializedJson = JsonSerializer.Serialize(message);

        JsonDocument jsonDocument = JsonDocument.Parse(serializedJson);
        var inboxMessage = new InboxMessage(
            messageId,
            message.GetType().AssemblyQualifiedName!,
            jsonDocument,
            DateTime.UtcNow
        );

        context.InboxMessages.Add(inboxMessage);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> FindDuplicate(Guid messageId)
    {
        return await context.InboxMessages.AnyAsync(m => m.Id == messageId);
    }
}