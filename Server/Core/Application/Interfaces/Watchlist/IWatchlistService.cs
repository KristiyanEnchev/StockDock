namespace Application.Interfaces.Watchlist
{
    using Models.Stock;

    using Shared;

    public interface IWatchlistService
    {
        Task<Result<List<UserWatchlistDto>>> GetUserWatchlistAsync(string userId);
        Task<Result<UserWatchlistDto>> AddToWatchlistAsync(string userId, string symbol);
        Task<Result<bool>> RemoveFromWatchlistAsync(string userId, string symbol);
    }
}