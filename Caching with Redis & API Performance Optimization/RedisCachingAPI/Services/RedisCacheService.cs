using StackExchange.Redis;
using System.Text.Json;

namespace RedisCachingAPI.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        _logger.LogInformation("Getting value from Redis with key: {Key}", key);
        
        var value = await _database.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            _logger.LogInformation("Cache miss for key: {Key}", key);
            return default;
        }

        _logger.LogInformation("Cache hit for key: {Key}", key);
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        _logger.LogInformation("Setting value in Redis with key: {Key}", key);
        
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiration ?? TimeSpan.FromMinutes(5));
    }

    public async Task RemoveAsync(string key)
    {
        _logger.LogInformation("Removing value from Redis with key: {Key}", key);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<long> ListPushAsync<T>(string key, T value)
    {
        _logger.LogInformation("Pushing value to Redis List with key: {Key}", key);
        var serializedValue = JsonSerializer.Serialize(value);
        return await _database.ListRightPushAsync(key, serializedValue);
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1)
    {
        _logger.LogInformation("Getting range from Redis List with key: {Key}", key);
        var values = await _database.ListRangeAsync(key, start, stop);
        
        return values
            .Where(v => !v.IsNullOrEmpty)
            .Select(v => JsonSerializer.Deserialize<T>(v!)!)
            .ToList();
    }

    public async Task<bool> SetAddAsync<T>(string key, T value)
    {
        _logger.LogInformation("Adding value to Redis Set with key: {Key}", key);
        var serializedValue = JsonSerializer.Serialize(value);
        return await _database.SetAddAsync(key, serializedValue);
    }

    public async Task<List<T>> SetMembersAsync<T>(string key)
    {
        _logger.LogInformation("Getting members from Redis Set with key: {Key}", key);
        var values = await _database.SetMembersAsync(key);
        
        return values
            .Where(v => !v.IsNullOrEmpty)
            .Select(v => JsonSerializer.Deserialize<T>(v!)!)
            .ToList();
    }

    public async Task<bool> HashSetAsync<T>(string key, string field, T value)
    {
        _logger.LogInformation("Setting field in Redis Hash with key: {Key}, field: {Field}", key, field);
        var serializedValue = JsonSerializer.Serialize(value);
        return await _database.HashSetAsync(key, field, serializedValue);
    }

    public async Task<T?> HashGetAsync<T>(string key, string field)
    {
        _logger.LogInformation("Getting field from Redis Hash with key: {Key}, field: {Field}", key, field);
        var value = await _database.HashGetAsync(key, field);
        
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
    {
        _logger.LogInformation("Getting all fields from Redis Hash with key: {Key}", key);
        var entries = await _database.HashGetAllAsync(key);
        
        var result = new Dictionary<string, T>();
        foreach (var entry in entries)
        {
            if (!entry.Value.IsNullOrEmpty)
            {
                result[entry.Name!] = JsonSerializer.Deserialize<T>(entry.Value!)!;
            }
        }
        
        return result;
    }

    public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
    {
        _logger.LogInformation("Adding value to Redis Sorted Set with key: {Key}, score: {Score}", key, score);
        var serializedValue = JsonSerializer.Serialize(value);
        return await _database.SortedSetAddAsync(key, serializedValue, score);
    }

    public async Task<List<T>> SortedSetRangeAsync<T>(string key, long start = 0, long stop = -1)
    {
        _logger.LogInformation("Getting range from Redis Sorted Set with key: {Key}", key);
        var values = await _database.SortedSetRangeByRankAsync(key, start, stop);
        
        return values
            .Where(v => !v.IsNullOrEmpty)
            .Select(v => JsonSerializer.Deserialize<T>(v!)!)
            .ToList();
    }
}
