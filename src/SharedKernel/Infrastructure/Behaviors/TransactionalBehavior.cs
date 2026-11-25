using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Infrastructure.Behaviors;
public abstract class TransactionalBehaviorBase<TRequest, TResponse, TContext>(TContext context) : 
                                        IPipelineBehavior<TRequest, TResponse> where TRequest : notnull where TContext : DbContext
{
    protected abstract bool AppliesTo(TRequest request);
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if(!AppliesTo(request))
            return await next(cancellationToken);

        var currentTransaction = context.Database.CurrentTransaction;

        if(context.Database.CurrentTransaction is not null)
        {
            var savepoint = $"SP_{typeof(TRequest).Name}_{Guid.NewGuid():N}";

            await currentTransaction!.CreateSavepointAsync(savepoint);

            try
            {
                var savePointResponse = await next(cancellationToken);
                await currentTransaction.ReleaseSavepointAsync(savepoint, cancellationToken);
                return savePointResponse;
            }
            catch(Exception ex)
            {
                await currentTransaction.RollbackToSavepointAsync(savepoint, cancellationToken);

                context.ChangeTracker.Clear();
                
                throw new Exception($"Savepoint rolled back: {savepoint}", ex);
            }
        }


        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        TResponse response = await next(cancellationToken);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return response;
    }
}