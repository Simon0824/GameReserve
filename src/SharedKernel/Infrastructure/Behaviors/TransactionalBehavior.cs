using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Infrastructure.Behaviors;
public abstract class TransactionalBehaviorBase<TRequest, TResponse, TContext>(
                                                TContext context, 
                                                ILogger<TransactionalBehaviorBase<TRequest, TResponse, TContext>> logger) : 
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

        using var transaction = await context.Database.BeginTransactionAsync();

        TResponse response = await next(cancellationToken);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return response;
    }
}