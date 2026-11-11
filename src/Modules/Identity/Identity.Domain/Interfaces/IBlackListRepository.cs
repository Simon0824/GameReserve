namespace Identity.Domain.Interfaces;
public interface IBlackListRepository
{
    Task<bool> BlackListUserAsync(string? userId, string? tokenId, TimeSpan? duration);
    Task<bool> IsBannedOrLoggedOutAsync(string userId, string tokenId);
}