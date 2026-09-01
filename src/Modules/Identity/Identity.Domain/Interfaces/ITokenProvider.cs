using Identity.Domain.UserAggregate;

namespace Identity.Domain.Interfaces;
public interface ITokenProvider
{
    string CreateToken(User user);
}