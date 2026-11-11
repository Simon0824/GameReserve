using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Interfaces;
public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string password);
    Task<User?> FindUserByEmail(string Email, CancellationToken cancellationToken);
    Task<User?> FindUser(string UserId, CancellationToken cancellationToken);
    Task<bool> CheckPassword(User user, string password);
    Task<IdentityResult> ChangePassword(User user, string CurrentPassword, string NewPassword);
    Task<IdentityResult> AddUserRole(User user);
    Task<IList<string>> GetUserRole(User user);
    Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken);
    Task<IdentityResult> DeleteUser(User user);
    void AddRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken?> FindRefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> FindRefreshTokenByUser(string userId, CancellationToken cancellationToken);
    Task RevokeRefreshTokenByUser(string userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}