using Reservations.Domain.Primitives;
using SharedKernel.Domain.Primivites.DomainEvent;

namespace Reservations.Domain.Aggregates;

public record ReservationCreatedDomainEvent(Guid Id, ReservationId ReservationId) : DomainEvent(Id);