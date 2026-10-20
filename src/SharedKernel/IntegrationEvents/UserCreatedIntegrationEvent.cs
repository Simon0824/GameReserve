namespace SharedKernel.IntegrationEvents;
public record UserCreatedIntegrationEvent(Guid UserId) : IntegrationEvent(Guid.CreateVersion7());