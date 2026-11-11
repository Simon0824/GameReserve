using Identity.Domain.Abstractions;
using Identity.Domain.UserAggregate;

namespace Identity.Domain.Entities;
public class RefreshToken
{
    public Guid Id {get; private set;}
    public required string UserId {get; init;}
    public required string Token {get; init;}
    public DateTime ExpiresOnUtc {get; private set;}
    public required User User {get; init;}

    public static RefreshToken CreateRefreshToken(string userId, string token, User user)
    {
        return new RefreshToken()
        {
          Id = Guid.NewGuid(),
          UserId = userId,
          Token = token,
          ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
          User = user  
        };
    }

    public void Revoke()
    {
        ExpiresOnUtc = DateTime.UtcNow.AddSeconds(-1);
    }
}