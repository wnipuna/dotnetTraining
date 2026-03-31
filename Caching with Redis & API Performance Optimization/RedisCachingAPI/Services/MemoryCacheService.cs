using Microsoft.Extensions.Caching.Memory;

namespace RedisCachingAPI.Services;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _logger.LogInformation("Getting value from Memory Cache with key: {Key}", key);
        
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation("Cache hit for key: {Key}", key);
            return Task.FromResult(value);
        }

        _logger.LogInformation("Cache miss for key: {Key}", key);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        _logger.LogInformation("Setting value in Memory Cache with key: {Key}", key);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
        }
        else
        {
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }

        _memoryCache.Set(key, value, cacheEntryOptions);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _logger.LogInformation("Removing value from Memory Cache with key: {Key}", key);
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
}
