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
                                            .OrderBy(m => m.OccuredOnTime)
                                            .Where(m => m.ProcessedOnTime == null)
                                            .Take(BatchSize)
                                            .ToListAsync();

        if(inboxMessages is null)
            return;
        
        foreach(var inboxMessage in inboxMessages)
        {
            try
            {
                var messageType = Type.GetType(inboxMessage.Type) 
                                                    ?? throw new Exception("Cannot get message type");

                var deseralizedMessage = JsonSerializer.Deserialize(inboxMessage.Content, messageType) 
                                                    ?? throw new Exception("Cannot desrialize a message");

                using(var scope = serviceProvider.CreateAsyncScope())
                {
                    var scopedPublish = scope.ServiceProvider.GetRequiredService<IPublisher>();

                    await scopedPublish.Publish(deseralizedMessage, cancellationToken);
                }

                inboxMessage.ProcessedOnTime = DateTime.UtcNow;

                inboxMessage.Error = null;

                await context.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                await context.Set<InboxMessage>()
                             .Where(m => m.Id == inboxMessage.Id)
                             .ExecuteUpdateAsync(m => 
                                m.SetProperty(p => p.Error, ex.Message), cancellationToken);
            }
        }
    }
}