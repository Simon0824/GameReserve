using Reservations.Application;
using Reservations.Infrastructure.Data;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Infrastructure.Behaviors;

namespace Reservations.Infrastructure.Behaviors;
public class ReservationsTransactionalBehavior<TRequest, TResponse>(ReservationsContext context) : 
                                                                    TransactionalBehaviorBase<TRequest, TResponse, ReservationsContext>(context) where TRequest : notnull
{
    protected override bool AppliesTo(TRequest request) => request is ICommandBase
                                                && typeof(TRequest).Assembly == typeof(ReservationsApplicationMarker).Assembly;
}