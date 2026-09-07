using SharedKernel.Domain.Primivites.DomainEvent;

namespace Identity.Domain.UserAggregate;
public record UserCreatedDomainEvent(Guid Id, Guid UserId) : DomainEvent(Id);