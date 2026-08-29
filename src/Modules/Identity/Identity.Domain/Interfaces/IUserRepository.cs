using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Interfaces;
public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string password);
}