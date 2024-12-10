namespace Application.Interfaces.Subscription
{
    using Shared;

    public interface IStockSubscriptionService
    {
        Task<Result<bool>> SubscribeAsync(string userId, string symbol);
        Task<Result<bool>> UnsubscribeAsync(string userId, string symbol);
        Task<Result<IReadOnlyList<string>>> GetActiveSubscriptionsAsync(string userId);
    }
}