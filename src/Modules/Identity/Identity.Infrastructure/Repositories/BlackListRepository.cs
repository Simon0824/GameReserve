using Identity.Domain.Interfaces;
using StackExchange.Redis;

namespace Identity.Infrastructure.Repositories;

public class BlackListRepository(IConnectionMultiplexer redis) : IBlackListRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    private const string PermamentSet = "id:blacklist:permament";

    public async Task<bool> BlockUserPermamentAsync(string? userId) => await _db.SetAddAsync(PermamentSet, userId);

    public async Task<bool> IsBannedAsync(string userId)
    {
        var isPermamentBan = await _db.SetContainsAsync(PermamentSet, userId);

        if(isPermamentBan)
            return true;
        
        return false;
    } 
}
