using MediatR;

namespace SharedKernel.IntegrationEvents;
public record PaymentFailedIntegrationEvent(Guid Id, Guid ReservationId) : INotification;