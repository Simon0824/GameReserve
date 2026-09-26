using MediatR;

namespace SharedKernel.IntegrationEvents;
public record ReservationCreatedIntegrationEvent(Guid Id, Guid ReservationId) : INotification;