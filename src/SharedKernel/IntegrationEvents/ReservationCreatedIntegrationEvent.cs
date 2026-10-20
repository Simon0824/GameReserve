using MediatR;

namespace SharedKernel.IntegrationEvents;
public record ReservationCreatedIntegrationEvent(Guid ReservationId, decimal Amount) : IntegrationEvent(Guid.CreateVersion7());