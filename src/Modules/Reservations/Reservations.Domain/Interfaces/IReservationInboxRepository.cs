namespace Reservations.Domain.Interfaces;
public interface IReservationsInboxRepository
{
    Task AddInboxMessage<T>(T message, Guid MessageId, CancellationToken cancellationToken) where T : notnull;
    Task<bool> FindDuplicate(Guid inboxId);
}