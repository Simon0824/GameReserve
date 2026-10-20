using MediatR;

namespace SharedKernel.IntegrationEvents;
public record PaymentConfirmedIntegrationEvent(Guid ReservationId) : IntegrationEvent(Guid.CreateVersion7());