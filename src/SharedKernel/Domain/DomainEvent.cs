using MediatR;

namespace SharedKernel.Domain.DomainEvent;
public record DomainEvent(Guid Id) : INotification;