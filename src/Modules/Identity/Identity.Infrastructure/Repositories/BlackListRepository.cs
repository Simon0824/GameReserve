using Identity.Domain.Interfaces;
using StackExchange.Redis;

namespace Identity.Infrastructure.Repositories;

public class BlackListRepository(IConnectionMultiplexer redis) : IBlackListRepository
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string PermamentSet = "id:blacklist:permament";
    private static string BlackListLogOutKey(string tokenId) => $"id:blacklist:{tokenId}";
    public async Task<bool> BlackListUserAsync(string? userId, string? tokenId, TimeSpan? duration)
    {
        if(!string.IsNullOrEmpty(tokenId))
        {
            return await _db.StringSetAsync(BlackListLogOutKey(tokenId), "revoked", duration);
        }

        if(!string.IsNullOrEmpty(userId))
        {
            return await _db.SetAddAsync(PermamentSet, userId);
        }

        return false;
    }

    public async Task<bool> IsBannedOrLoggedOutAsync(string userId, string tokenId)
    {
        var isPermamentBan = await _db.SetContainsAsync(PermamentSet, userId);

        var loggedOut = await _db.StringGetAsync(BlackListLogOutKey(tokenId));

        if(isPermamentBan || !string.IsNullOrEmpty(loggedOut))
            return true;
        
        return false;
    } 
}
