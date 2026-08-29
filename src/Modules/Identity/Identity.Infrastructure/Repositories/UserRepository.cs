using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories;
public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }
}