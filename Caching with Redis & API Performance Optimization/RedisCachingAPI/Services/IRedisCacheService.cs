namespace RedisCachingAPI.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<long> ListPushAsync<T>(string key, T value);
    Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1);
    Task<bool> SetAddAsync<T>(string key, T value);
    Task<List<T>> SetMembersAsync<T>(string key);
    Task<bool> HashSetAsync<T>(string key, string field, T value);
    Task<T?> HashGetAsync<T>(string key, string field);
    Task<Dictionary<string, T>> HashGetAllAsync<T>(string key);
    Task<bool> SortedSetAddAsync<T>(string key, T value, double score);
    Task<List<T>> SortedSetRangeAsync<T>(string key, long start = 0, long stop = -1);
}
