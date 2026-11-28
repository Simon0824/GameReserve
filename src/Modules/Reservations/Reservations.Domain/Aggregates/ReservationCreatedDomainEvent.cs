using Reservations.Domain.Primitives;
using SharedKernel.Domain.Enums;
using SharedKernel.Domain.Primivites.DomainEvent;

namespace Reservations.Domain.Aggregates;

public record ReservationCreatedDomainEvent(Guid Id, ReservationId ReservationId, decimal Amount, decimal CalculatedPrice, PaymentCurrency Currency, CardTypes Card) : DomainEvent(Id);