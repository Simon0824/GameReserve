namespace Identity.Domain.Interfaces;
public interface IBlackListRepository
{
    Task<bool> BlockUserAsync(string? userId, string? tokenId, string reason = "manual", TimeSpan? duration = null);
    Task<bool> IsBannedAsync(string userId);
}