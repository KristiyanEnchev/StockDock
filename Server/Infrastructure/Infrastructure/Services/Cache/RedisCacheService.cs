namespace Infrastructure.Services.Cache
{
    using System.Text.Json;

    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Caching.Distributed;

    using StackExchange.Redis;

    using Models;

    using Application.Interfaces.Cache;

    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly RedisSettings _settings;

        public RedisCacheService(
            IConnectionMultiplexer redis,
            IDistributedCache cache,
            IOptions<RedisSettings> settings,
            JsonSerializerOptions jsonOptions)
        {
            _redis = redis;
            _cache = cache;
            _jsonOptions = jsonOptions;
            _settings = settings.Value;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes)
            };

            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, jsonValue, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            var subscriber = _redis.GetSubscriber();
            var jsonMessage = JsonSerializer.Serialize(message, _jsonOptions);
            await subscriber.PublishAsync(channel, jsonMessage);
        }

        public async Task SubscribeAsync<T>(string channel, Func<T, Task> handler)
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync(channel, async (_, message) =>
            {
                var value = JsonSerializer.Deserialize<T>(message!, _jsonOptions);
                if (value != null)
                {
                    await handler(value);
                }
            });
        }
    }
}