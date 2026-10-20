namespace Reservations.Domain.Interfaces;
public interface IReservationsInboxRepository
{
    Task<bool> FindDuplicate(Guid inboxId);
    Task AddInboxMessage<T>(T message, Guid MessageId, CancellationToken cancellationToken) where T : notnull;
}