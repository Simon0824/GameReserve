using MediatR;

namespace SharedKernel.IntegrationEvents;
public record IntegrationEvent(Guid Id) : INotification;