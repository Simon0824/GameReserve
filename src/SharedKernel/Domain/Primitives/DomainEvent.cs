using MediatR;

namespace SharedKernel.Domain.Primivites.DomainEvent;
public record DomainEvent(Guid Id) : INotification;