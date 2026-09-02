using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Interfaces;
public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string password);
    Task<User?> FindUser(string email);
    Task<bool> CheckPassword(User user, string password);
    Task<IdentityResult> AddUserRole(User user);
    Task<IdentityResult> DeleteUser(User user);
}