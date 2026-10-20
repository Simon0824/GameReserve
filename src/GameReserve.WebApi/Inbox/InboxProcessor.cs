using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Abstractions;

namespace GemeReserve.WebApi.Inbox;
public class InboxProcessor<TContext>(TContext context, IServiceProvider serviceProvider) where TContext : DbContext
{
    private const int BatchSize = 10;
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var inboxMessages = await context.Set<InboxMessage>()
                                    .Where(m => m.ProcessedOnTime == null)
                                    .OrderBy(m => m.OccuredOnTime)
                                    .Take(BatchSize)
                                    .ToListAsync();

        if(inboxMessages.Count() == 0)
        {
            return;
        }

        foreach(var message in inboxMessages)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
            var messageType = Type.GetType(message.Type)  
                                        ?? throw new Exception("Cannot get type of inbox message");
            var deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType)
                                        ?? throw new Exception("Cannot deserialize inbox content");

            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                var scopePublisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                await scopePublisher.Publish(deserializedMessage, cancellationToken);
            }

            message.ProcessedOnTime = DateTime.UtcNow;
            message.Error = null;

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                    await context.Set<InboxMessage>()
                                 .Where(m => m.Id == message.Id)
                                 .ExecuteUpdateAsync(m => 
                                                     m.SetProperty(e => e.Error, ex.Message), cancellationToken);
            }

        }
    }
}