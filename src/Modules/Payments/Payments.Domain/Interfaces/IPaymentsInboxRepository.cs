namespace Payments.Domain.Interfaces;
public interface IPaymentsInboxRepository
{
    Task AddInboxMessage<T>(T message, Guid messageId, CancellationToken cancellationToken) where T : notnull;
    Task<bool> FindDuplicate(Guid messageId);
}