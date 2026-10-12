namespace Payments.Domain.Interfaces;
public interface IPaymentsOutboxRepository
{
    void InsertOutboxMessage<T>(T message) where T : notnull;
}