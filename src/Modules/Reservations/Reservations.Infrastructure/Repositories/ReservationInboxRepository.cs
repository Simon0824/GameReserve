using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Reservations.Domain.Interfaces;
using Reservations.Infrastructure.Data;
using SharedKernel.Domain.Abstractions;

namespace Reservations.Infrastructure.Repositories;
public class ReservationInboxRepository(ReservationsContext context) : IReservationsInboxRepository
{
    public async Task AddInboxMessage<T>(T message, Guid MessageId, CancellationToken cancellationToken) where T : notnull
    {
        var jsonString = JsonSerializer.Serialize(message);

        JsonDocument serializedJson = JsonDocument.Parse(jsonString);
        var inboxMessage = new InboxMessage(
            MessageId, 
            message.GetType().AssemblyQualifiedName!,
            serializedJson,
            DateTime.UtcNow);

        context.InboxMessages.Add(inboxMessage);

        await context.SaveChangesAsync();
    }

    public async Task<bool> FindDuplicate(Guid inboxId)
    {
        return await context.InboxMessages.AnyAsync(m => m.Id == inboxId);
    }
}