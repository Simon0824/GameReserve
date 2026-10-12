using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.WebApi.Outbox;
public class OutboxProcessor<TContext>(TContext context, IServiceProvider serviceProvider) where TContext : DbContext
{
    private const int BatchSize = 10;
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var messages = await context.Set<OutboxMessage>()
                                .Where(m => m.ProcessedOnTime == null)
                                .OrderBy(m => m.OccuredOnTime)
                                .Take(BatchSize)
                                .ToListAsync();

        if(messages.Count() == 0)
        {
            return;
        }

        foreach(var message in messages)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
            var messageType = Type.GetType(message.Type) ?? throw new Exception("Cannot get a message type");

            var deseralizedMessage = JsonSerializer.Deserialize(message.Content, messageType) ?? throw new Exception("Cannot deserialize message");     

            using(var scope = serviceProvider.CreateAsyncScope())
            {
            var scopePublishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            await scopePublishEndpoint.Publish(deseralizedMessage, messageType, cancellationToken);                        
            }

            message.ProcessedOnTime = DateTime.UtcNow;
            message.Error = null;
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();      
            }
            catch(Exception ex)
            {
                transaction.Rollback();

                await context.Set<OutboxMessage>()
                                .Where(m => m.Id == message.Id)
                                .ExecuteUpdateAsync(m => 
                                m.SetProperty(m => m.Error, ex.Message), cancellationToken);
            }
        }
    }
}