using Identity.Domain.Constants;
using Identity.Domain.Entites;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories;
public class UserRepository(UserManager<User> userManager, IdentityContext context) : IUserRepository
{
    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<User?> FindUser(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<bool> CheckPassword(User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> AddUserRole(User user)
    {
        return await userManager.AddToRoleAsync(user, UserRoles.User);
    }
    public async Task<IdentityResult> DeleteUser(User user)
    {
        return await userManager.DeleteAsync(user);
    }

    public async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await context.refreshTokens.AddAsync(refreshToken);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}