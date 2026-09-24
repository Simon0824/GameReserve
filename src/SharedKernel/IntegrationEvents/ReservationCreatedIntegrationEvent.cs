using MediatR;
using SharedKernel.Domain.Primivites.DomainEvent;

namespace SharedKernel.IntegrationEvents;
public record ReservationCreatedIntegrationEvent(Guid Id, Guid ReservationId) : INotification;