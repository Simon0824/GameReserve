namespace Identity.Domain.Interfaces;
public interface IIdentityOutboxRepository
{
    void InsertOutboxMessage<T>(T message) where T : notnull;
}