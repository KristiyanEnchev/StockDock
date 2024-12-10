namespace Application.Interfaces.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task PublishAsync<T>(string channel, T message);
        Task SubscribeAsync<T>(string channel, Func<T, Task> handler);
    }
}