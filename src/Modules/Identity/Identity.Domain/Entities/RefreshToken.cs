using Identity.Domain.UserAggregate;

namespace Identity.Domain.Entities;
public class RefreshToken
{
    public Guid Id {get; set;}
    public required string UserId {get; set;}
    public required string Token {get; set;}
    public DateTime ExpiresOnUtc {get; set;}
    public required User User {get; set;}
}