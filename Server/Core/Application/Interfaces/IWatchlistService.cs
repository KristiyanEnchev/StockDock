namespace Application.Interfaces
{
    using Models.Stock;

    using Shared;

    public interface IWatchlistService
    {
        Task<Result<UserWatchlistDto>> AddToWatchlistAsync(string userId, CreateWatchlistItemRequest request);
        Task<Result<bool>> RemoveFromWatchlistAsync(string userId, string symbol);
        Task<Result<IReadOnlyList<UserWatchlistDto>>> GetUserWatchlistAsync(string userId);
        Task<Result<bool>> UpdateWatchlistAlertsAsync(string userId, string symbol, decimal? alertAbove, decimal? alertBelow);
    }
}