using Identity.Infrastructure.Data;
using Identy.Application.IdentityAppliactionMarker;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Infrastructure.Behaviors;
namespace Identity.Infrastructure.Behaviors;
public sealed class IdentityTransactionalBehavior<TRequest, TResponse>(
                                                            IdentityContext context, 
                                                            ILogger<IdentityTransactionalBehavior<TRequest, TResponse>> logger) : 
                                                TransactionalBehaviorBase<TRequest, TResponse, IdentityContext>(context, logger) where TRequest :  notnull
{
    protected override bool AppliesTo(TRequest request) => request is ICommandBase 
                            && typeof(TRequest).Assembly == typeof(IdentityApplicationMarker).Assembly;
}