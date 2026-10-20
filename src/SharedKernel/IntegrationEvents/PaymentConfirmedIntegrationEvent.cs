using MediatR;

namespace SharedKernel.IntegrationEvents;
public record PaymentFailedIntegrationEvent(Guid ReservationId) : IntegrationEvent(Guid.CreateVersion7());