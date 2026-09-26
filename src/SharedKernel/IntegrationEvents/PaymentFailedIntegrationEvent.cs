using MediatR;

namespace SharedKernel.IntegrationEvents;
public record PaymentConfirmedIntegrationEvent(Guid Id, Guid ReservationId) : INotification;