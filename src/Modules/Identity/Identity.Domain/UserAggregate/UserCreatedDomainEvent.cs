using Identity.Domain.Primitives;

namespace Identity.Domain.UserAggregate;
public record UserCreatedDomainEvent(Guid Id, Guid UserId) : DomainEvent(Id);