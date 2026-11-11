namespace Identity.Domain.Interfaces;
public interface IBlackListRepository
{
    Task<bool> BlockUserPermamentAsync(string? userId);
    Task<bool> IsBannedAsync(string userId);
}