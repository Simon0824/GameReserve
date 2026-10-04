using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Interfaces;
public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string password);
    Task<User?> FindUserByEmail(string Email);
    Task<User?> FindUser(string UserId);
    Task<bool> CheckPassword(User user, string password);
    Task<IdentityResult> AddUserRole(User user);
    Task<IList<string>> GetUserRole(User user);
    Task<IEnumerable<User>> GetUsers();
    Task<IdentityResult> DeleteUser(User user);
    void AddRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken?> FindRefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task UpdateAsync(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}