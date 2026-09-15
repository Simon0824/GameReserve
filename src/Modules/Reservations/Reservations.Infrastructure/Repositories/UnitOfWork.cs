using Reservations.Domain.Interfaces;
using Reservations.Infrastructure.Data;

namespace Reservations.Infrastructure.Repositories;
public class UnitOfWork(ReservationsContext context) : IUnitOfWork
{

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}