using Payments.Application;
using Payments.Infrastructure.Data;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Infrastructure.Behaviors;

namespace Payments.Infrastructure.Behaviors;
public sealed class PaymentsTransactionalBehavior<TRequest, TResponse>(PaymentsContext context) : 
                                                TransactionalBehaviorBase<TRequest, TResponse, PaymentsContext>(context) where TRequest :  notnull
{
    protected override bool AppliesTo(TRequest request) => request is ICommandBase 
                            && typeof(TRequest).Assembly == typeof(PaymentsApplicationMarker).Assembly;
}