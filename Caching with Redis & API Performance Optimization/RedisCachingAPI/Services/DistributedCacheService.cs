using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisCachingAPI.Services;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCacheService> _logger;

    public DistributedCacheService(IDistributedCache distributedCache, ILogger<DistributedCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        _logger.LogInformation("Getting value from Distributed Cache with key: {Key}", key);
        
        var cachedData = await _distributedCache.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache miss for key: {Key}", key);
            return default;
        }

        _logger.LogInformation("Cache hit for key: {Key}", key);
        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        _logger.LogInformation("Setting value in Distributed Cache with key: {Key}", key);
        
        var options = new DistributedCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }
        else
        {
            options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }

        var serializedData = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedData, options);
    }

    public async Task RemoveAsync(string key)
    {
        _logger.LogInformation("Removing value from Distributed Cache with key: {Key}", key);
        await _distributedCache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var cachedData = await _distributedCache.GetStringAsync(key);
        return !string.IsNullOrEmpty(cachedData);
    }
}
