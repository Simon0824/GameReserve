namespace Reservations.Domain.Interfaces;
public interface IReservationsOutboxRepository
{
    void InsertOutboxMessage<T>(T message) where T : notnull;
}