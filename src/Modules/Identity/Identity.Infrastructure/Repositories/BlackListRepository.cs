using Identity.Domain.Interfaces;
using StackExchange.Redis;

namespace Identity.Infrastructure.Repositories;

public class BlackListRepository(IConnectionMultiplexer redis) : IBlackListRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    private const string PermamentSet = "id:blacklist:permament";
    private static string BlackListKey(string tokenId) => $"id:blacklist:{tokenId}";

    public async Task<bool> BlockUserAsync(string? userId, string? tokenId, string reason = "manual", TimeSpan? duration = null)
    {
        var tx = _db.CreateTransaction();
        if(userId == null && tokenId != null)
        {
            _ = tx.StringSetAsync(BlackListKey(tokenId), reason, duration);
        }        
        else
        {
            _ = tx.StringSetAsync(BlackListKey(tokenId!), reason, duration);
            _ = tx.SetAddAsync(PermamentSet, userId);
        }

        return await tx.ExecuteAsync();
    } 

    public async Task<bool> IsBannedAsync(string userId)
    {
        var isPermamentBan = await _db.SetContainsAsync(PermamentSet, userId);

        if(isPermamentBan)
            return true;
        
        return false;
    } 
}
