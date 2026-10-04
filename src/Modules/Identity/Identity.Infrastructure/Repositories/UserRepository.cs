using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Constants;

namespace Identity.Infrastructure.Repositories;
public class UserRepository(UserManager<User> userManager, IdentityContext context) : IUserRepository
{
    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<User?> FindUserByEmail(string Email)
    {
        return await userManager.FindByEmailAsync(Email);
    }

    public async Task<User?> FindUser(string UserId)
    {
        return await userManager.FindByIdAsync(UserId);
    }

    public async Task<bool> CheckPassword(User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> ChangePassword(User user, string CurrentPassword, string NewPassword)
    {
        return await userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
    }
    public async Task<IdentityResult> AddUserRole(User user)
    {
        return await userManager.AddToRoleAsync(user, UserRoles.User);
    }

    public async Task<IList<string>> GetUserRole(User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await userManager.Users.AsNoTracking().ToListAsync();
    }
    public async Task<IdentityResult> DeleteUser(User user)
    {
        return await userManager.DeleteAsync(user);
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        context.refreshTokens.Add(refreshToken);
    }

    public async Task<RefreshToken?> FindRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        return await context.refreshTokens
                     .Include(u => u.User)
                     .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);
    }

    public async Task UpdateAsync(User user)
    {
        await userManager.UpdateAsync(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}