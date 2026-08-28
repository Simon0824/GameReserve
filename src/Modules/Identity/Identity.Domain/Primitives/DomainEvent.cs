using MediatR;

namespace Identity.Domain.Primitives;
public record DomainEvent(Guid Id) : INotification;