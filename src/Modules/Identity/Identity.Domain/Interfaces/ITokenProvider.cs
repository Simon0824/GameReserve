using Identity.Domain.UserAggregate;

namespace Identity.Domain.Interfaces;
public interface ITokenProvider
{
    Task<string> CreateToken(User user);
    string GenerateRefreshToken();
}