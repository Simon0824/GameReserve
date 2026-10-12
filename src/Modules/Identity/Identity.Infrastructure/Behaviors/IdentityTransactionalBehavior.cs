using Identity.Infrastructure.Data;
using Identy.Application.IdentityAppliactionMarker;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Infrastructure.Behaviors;
namespace Identity.Infrastructure.Behaviors;
public sealed class IdentityTransactionalBehavior<TRequest, TResponse>(IdentityContext context) : 
                                                TransactionalBehaviorBase<TRequest, TResponse, IdentityContext>(context) where TRequest :  notnull
{
    protected override bool AppliesTo(TRequest request) => request is ICommandBase 
                            && typeof(TRequest).Assembly == typeof(IdentityApplicationMarker).Assembly;
}