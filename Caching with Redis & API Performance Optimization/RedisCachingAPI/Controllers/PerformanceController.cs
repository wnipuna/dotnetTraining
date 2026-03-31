using Microsoft.AspNetCore.Mvc;
using RedisCachingAPI.Services;
using System.Diagnostics;

namespace RedisCachingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceController : ControllerBase
{
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IDistributedCacheService _distributedCacheService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ILogger<PerformanceController> _logger;

    public PerformanceController(
        IMemoryCacheService memoryCacheService,
        IDistributedCacheService distributedCacheService,
        IRedisCacheService redisCacheService,
        ILogger<PerformanceController> logger)
    {
        _memoryCacheService = memoryCacheService;
        _distributedCacheService = distributedCacheService;
        _redisCacheService = redisCacheService;
        _logger = logger;
    }

    [HttpGet("compare/{id}")]
    public async Task<ActionResult> CompareCachingStrategies(int id)
    {
        var results = new Dictionary<string, object>();

        var memoryStopwatch = Stopwatch.StartNew();
        await TestMemoryCache(id);
        memoryStopwatch.Stop();
        results["Memory Cache"] = new { timeMs = memoryStopwatch.ElapsedMilliseconds };

        var distributedStopwatch = Stopwatch.StartNew();
        await TestDistributedCache(id);
        distributedStopwatch.Stop();
        results["Distributed Cache"] = new { timeMs = distributedStopwatch.ElapsedMilliseconds };

        var redisStopwatch = Stopwatch.StartNew();
        await TestRedisCache(id);
        redisStopwatch.Stop();
        results["Redis Cache"] = new { timeMs = redisStopwatch.ElapsedMilliseconds };

        return Ok(new
        {
            message = "Performance comparison completed",
            results,
            recommendation = GetRecommendation(results)
        });
    }

    [HttpGet("async-demo")]
    public async Task<ActionResult> AsyncPerformanceDemo()
    {
        var stopwatch = Stopwatch.StartNew();

        var task1 = SimulateLongRunningOperation("Operation 1", 1000);
        var task2 = SimulateLongRunningOperation("Operation 2", 1500);
        var task3 = SimulateLongRunningOperation("Operation 3", 2000);

        var results = await Task.WhenAll(task1, task2, task3);
        
        stopwatch.Stop();

        return Ok(new
        {
            message = "Asynchronous operations completed",
            totalTimeMs = stopwatch.ElapsedMilliseconds,
            results,
            note = "All operations ran concurrently, saving time compared to sequential execution"
        });
    }

    [HttpGet("sync-vs-async")]
    public async Task<ActionResult> CompareSyncVsAsync()
    {
        var syncStopwatch = Stopwatch.StartNew();
        await SimulateLongRunningOperation("Sync 1", 500);
        await SimulateLongRunningOperation("Sync 2", 500);
        await SimulateLongRunningOperation("Sync 3", 500);
        syncStopwatch.Stop();
        var syncTime = syncStopwatch.ElapsedMilliseconds;

        var asyncStopwatch = Stopwatch.StartNew();
        var tasks = new[]
        {
            SimulateLongRunningOperation("Async 1", 500),
            SimulateLongRunningOperation("Async 2", 500),
            SimulateLongRunningOperation("Async 3", 500)
        };
        await Task.WhenAll(tasks);
        asyncStopwatch.Stop();
        var asyncTime = asyncStopwatch.ElapsedMilliseconds;

        return Ok(new
        {
            sequentialTimeMs = syncTime,
            parallelTimeMs = asyncTime,
            timeSavedMs = syncTime - asyncTime,
            performanceImprovement = $"{((syncTime - asyncTime) / (double)syncTime * 100):F2}%"
        });
    }

    private async Task<string> TestMemoryCache(int id)
    {
        var key = $"perf_test_memory_{id}";
        var cached = await _memoryCacheService.GetAsync<string>(key);
        
        if (cached == null)
        {
            await Task.Delay(100);
            await _memoryCacheService.SetAsync(key, $"Data for {id}", TimeSpan.FromMinutes(5));
            return "Cache Miss";
        }
        
        return "Cache Hit";
    }

    private async Task<string> TestDistributedCache(int id)
    {
        var key = $"perf_test_distributed_{id}";
        var cached = await _distributedCacheService.GetAsync<string>(key);
        
        if (cached == null)
        {
            await Task.Delay(100);
            await _distributedCacheService.SetAsync(key, $"Data for {id}", TimeSpan.FromMinutes(5));
            return "Cache Miss";
        }
        
        return "Cache Hit";
    }

    private async Task<string> TestRedisCache(int id)
    {
        var key = $"perf_test_redis_{id}";
        var cached = await _redisCacheService.GetAsync<string>(key);
        
        if (cached == null)
        {
            await Task.Delay(100);
            await _redisCacheService.SetAsync(key, $"Data for {id}", TimeSpan.FromMinutes(5));
            return "Cache Miss";
        }
        
        return "Cache Hit";
    }

    private async Task<string> SimulateLongRunningOperation(string operationName, int delayMs)
    {
        _logger.LogInformation("Starting {Operation}", operationName);
        await Task.Delay(delayMs);
        _logger.LogInformation("Completed {Operation}", operationName);
        return $"{operationName} completed in {delayMs}ms";
    }

    private string GetRecommendation(Dictionary<string, object> results)
    {
        return "Memory Cache is fastest for single-server scenarios. " +
               "Distributed/Redis Cache is recommended for multi-server deployments and data persistence.";
    }
}
