using SharedKernel.Domain.Enums;

namespace SharedKernel.IntegrationEvents;
public record ReservationCreatedIntegrationEvent(Guid ReservationId, decimal Amount, PaymentCurrency Currency, CardTypes Card) : IntegrationEvent(Guid.CreateVersion7());