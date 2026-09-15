namespace Reservations.Domain.Interfaces;
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}