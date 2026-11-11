using System.Text.Json;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Infrastructure.Repositories;
public class CachedUserRepository(IDistributedCache cache, IUserRepository decorated) : IUserRepository
{
    private const string _cachedUsers = "users:all";
    private static string RefreshTokenKey(string token) => $"refreshToken:{token}";
    private static string RefreshTokenByUserIdKey(string userId) => $"refreshToken:{userId}";
    private static string UsersByEmailKey(string email) => $"users:{email}";
    private static string UsersByIdKey(string id) => $"users:{id}";

    DistributedCacheEntryOptions UsersTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    DistributedCacheEntryOptions UsersByEmailTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };
    DistributedCacheEntryOptions UsersByIdTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };
    DistributedCacheEntryOptions RefreshTokenTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(6)
    };
    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        var isSuccess = await decorated.CreateUser(user, password);
        if(isSuccess.Succeeded)
        {
            await cache.RemoveAsync(_cachedUsers);
        }

        return isSuccess;
    }

    public async Task<User?> FindUserByEmail(string email, CancellationToken cancellationToken)
    {
        var cachedUser = await cache.GetStringAsync(UsersByEmailKey(email), cancellationToken);

        User? user;
        if(string.IsNullOrEmpty(cachedUser))
        {
            user = await decorated.FindUserByEmail(email, cancellationToken);
            if(user == null)
                return user!;

            await cache.SetStringAsync(UsersByEmailKey(email), JsonSerializer.Serialize(user), UsersByEmailTtl, cancellationToken);

            return user;
        }

        var deserializedUser = JsonSerializer.Deserialize<User?>(cachedUser!);

        user = deserializedUser!;
        return user;
    }

    public async Task<User?> FindUser(string userId, CancellationToken cancellationToken)
    {
        var cachedUser = await cache.GetStringAsync(UsersByIdKey(userId), cancellationToken);

        User? user;
        if(string.IsNullOrEmpty(cachedUser))
        {
            user = await decorated.FindUser(userId, cancellationToken);
            if(user == null)
                return user!;

            await cache.SetStringAsync(UsersByIdKey(userId), JsonSerializer.Serialize(user), UsersByIdTtl, cancellationToken);

            return user;
        }

        var deserializedUser = JsonSerializer.Deserialize<User?>(cachedUser!);

        user = deserializedUser!;
        return user;
    }

    public async Task<bool> CheckPassword(User user, string password)
    {
        return await decorated.CheckPassword(user, password);
    }

    public async Task<IdentityResult> ChangePassword(User user, string currentPassword, string newPassword)
    {
        var changeSuccess = await decorated.ChangePassword(user, currentPassword, newPassword);
        if(changeSuccess.Succeeded)
                await InvalidateUserAsync(user);

        return changeSuccess;
    }
    public async Task<IdentityResult> AddUserRole(User user)
    {
        return await decorated.AddUserRole(user);
    }

    public async Task<IList<string>> GetUserRole(User user)
    {
        return await decorated.GetUserRole(user);
    }

    public async Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken)
    {
        var cachedUsers = await cache.GetStringAsync(_cachedUsers, cancellationToken);

        IEnumerable<User> users;
        if(string.IsNullOrEmpty(cachedUsers))
        {
            users = await decorated.GetUsers(cancellationToken);
            if(users == null)
                return users!;

            await cache.SetStringAsync(_cachedUsers, JsonSerializer.Serialize(users), UsersTtl, cancellationToken);

            return users;
        }

        var deserializedUsers = JsonSerializer.Deserialize<IEnumerable<User>>(cachedUsers!);

        users = deserializedUsers!;
        return users;
    }
    public async Task<IdentityResult> DeleteUser(User user)
    {
        var deleteSuccess = await decorated.DeleteUser(user);
        if(deleteSuccess.Succeeded)
                await InvalidateUserAsync(user);
        return deleteSuccess;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        decorated.AddRefreshToken(refreshToken);
    }

    public async Task<RefreshToken?> FindRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var cachedToken = await cache.GetStringAsync(RefreshTokenKey(refreshToken), cancellationToken);

        RefreshToken? token;
        if(string.IsNullOrEmpty(cachedToken))
        {
            token = await decorated.FindRefreshToken(refreshToken, cancellationToken);
            if(token == null)
            {
                return token;
            }

            await cache.SetStringAsync(RefreshTokenKey(refreshToken), JsonSerializer.Serialize(token), RefreshTokenTtl, cancellationToken);

            return token;
        }

        var deserializedToken = JsonSerializer.Deserialize<RefreshToken?>(cachedToken!);

        token = deserializedToken;
        return token;
    }

    public async Task<RefreshToken?> FindRefreshTokenByUser(string userId, CancellationToken cancellationToken)
    {
        var cachedToken = await cache.GetStringAsync(RefreshTokenByUserIdKey(userId), cancellationToken);

        RefreshToken? token;
        if(string.IsNullOrEmpty(cachedToken))
        {
            token = await decorated.FindRefreshTokenByUser(userId, cancellationToken);
            if(token == null)
            {
                return token;
            }

            await cache.SetStringAsync(RefreshTokenByUserIdKey(userId), JsonSerializer.Serialize(token), RefreshTokenTtl, cancellationToken);

            return token;
        }

        var deserializedToken = JsonSerializer.Deserialize<RefreshToken?>(cachedToken!);

        token = deserializedToken;
        return token;
    }

    public async Task RevokeRefreshTokenByUser(string userId, CancellationToken cancellationToken)
    {
        await decorated.RevokeRefreshTokenByUser(userId, cancellationToken);
        await cache.RemoveAsync(RefreshTokenByUserIdKey(userId));
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await decorated.SaveChangesAsync(cancellationToken);
    }

    private async Task InvalidateUserAsync(User user)
    {
        await cache.RemoveAsync(_cachedUsers);
        await cache.RemoveAsync(UsersByIdKey(user.Id));

        if(!string.IsNullOrEmpty(user.Email))
                await cache.RemoveAsync(UsersByEmailKey(user.Email));
    }
}