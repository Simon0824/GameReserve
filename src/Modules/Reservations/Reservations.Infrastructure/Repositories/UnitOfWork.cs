using Reservations.Infrastructure.Data;

namespace Reservations.Infrastructure.Repositories;
public class UnitOfWork(ReservationsContext context)
{

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}