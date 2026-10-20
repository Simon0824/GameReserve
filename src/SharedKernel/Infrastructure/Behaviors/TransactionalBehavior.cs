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

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            
        TResponse response = await next(cancellationToken);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return response;
        }
        catch(Exception)
        {
            throw;
        }
    }
}