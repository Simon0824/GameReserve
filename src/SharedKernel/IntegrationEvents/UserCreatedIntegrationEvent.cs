namespace SharedKernel.IntegrationEvents;
public record UserCreatedIntegrationEvent(Guid Id, Guid UserId);