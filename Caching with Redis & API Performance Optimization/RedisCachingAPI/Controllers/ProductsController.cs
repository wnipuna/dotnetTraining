using Microsoft.AspNetCore.Mvc;
using RedisCachingAPI.Models;
using RedisCachingAPI.Services;

namespace RedisCachingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IDistributedCacheService _distributedCacheService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IMemoryCacheService memoryCacheService,
        IDistributedCacheService distributedCacheService,
        IRedisCacheService redisCacheService,
        ILogger<ProductsController> logger)
    {
        _memoryCacheService = memoryCacheService;
        _distributedCacheService = distributedCacheService;
        _redisCacheService = redisCacheService;
        _logger = logger;
    }

    [HttpGet("memory/{id}")]
    public async Task<ActionResult<Product>> GetProductWithMemoryCache(int id)
    {
        var cacheKey = $"product_memory_{id}";
        
        var cachedProduct = await _memoryCacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            _logger.LogInformation("Product {Id} retrieved from Memory Cache", id);
            return Ok(new { source = "Memory Cache", data = cachedProduct });
        }

        var product = await SimulateDbQueryAsync(id);
        await _memoryCacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        
        _logger.LogInformation("Product {Id} retrieved from Database and cached in Memory", id);
        return Ok(new { source = "Database (cached in Memory)", data = product });
    }

    [HttpGet("distributed/{id}")]
    public async Task<ActionResult<Product>> GetProductWithDistributedCache(int id)
    {
        var cacheKey = $"product_distributed_{id}";
        
        var cachedProduct = await _distributedCacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            _logger.LogInformation("Product {Id} retrieved from Distributed Cache", id);
            return Ok(new { source = "Distributed Cache", data = cachedProduct });
        }

        var product = await SimulateDbQueryAsync(id);
        await _distributedCacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        
        _logger.LogInformation("Product {Id} retrieved from Database and cached in Distributed Cache", id);
        return Ok(new { source = "Database (cached in Distributed)", data = product });
    }

    [HttpGet("redis/{id}")]
    public async Task<ActionResult<Product>> GetProductWithRedisCache(int id)
    {
        var cacheKey = $"product_redis_{id}";
        
        var cachedProduct = await _redisCacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            _logger.LogInformation("Product {Id} retrieved from Redis Cache", id);
            return Ok(new { source = "Redis Cache", data = cachedProduct });
        }

        var product = await SimulateDbQueryAsync(id);
        await _redisCacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        
        _logger.LogInformation("Product {Id} retrieved from Database and cached in Redis", id);
        return Ok(new { source = "Database (cached in Redis)", data = product });
    }

    [HttpPost("redis/list")]
    public async Task<ActionResult> AddProductToList([FromBody] Product product)
    {
        var listKey = "products_list";
        await _redisCacheService.ListPushAsync(listKey, product);
        
        return Ok(new { message = "Product added to Redis List", key = listKey });
    }

    [HttpGet("redis/list")]
    public async Task<ActionResult<List<Product>>> GetProductsList()
    {
        var listKey = "products_list";
        var products = await _redisCacheService.ListRangeAsync<Product>(listKey);
        
        return Ok(new { source = "Redis List", count = products.Count, data = products });
    }

    [HttpPost("redis/set")]
    public async Task<ActionResult> AddProductToSet([FromBody] Product product)
    {
        var setKey = "products_set";
        var added = await _redisCacheService.SetAddAsync(setKey, product);
        
        return Ok(new { message = added ? "Product added to Redis Set" : "Product already exists in Set", key = setKey });
    }

    [HttpGet("redis/set")]
    public async Task<ActionResult<List<Product>>> GetProductsSet()
    {
        var setKey = "products_set";
        var products = await _redisCacheService.SetMembersAsync<Product>(setKey);
        
        return Ok(new { source = "Redis Set", count = products.Count, data = products });
    }

    [HttpPost("redis/hash/{productId}")]
    public async Task<ActionResult> AddProductToHash(int productId, [FromBody] Product product)
    {
        var hashKey = "products_hash";
        await _redisCacheService.HashSetAsync(hashKey, productId.ToString(), product);
        
        return Ok(new { message = "Product added to Redis Hash", key = hashKey, field = productId });
    }

    [HttpGet("redis/hash/{productId}")]
    public async Task<ActionResult<Product>> GetProductFromHash(int productId)
    {
        var hashKey = "products_hash";
        var product = await _redisCacheService.HashGetAsync<Product>(hashKey, productId.ToString());
        
        if (product == null)
        {
            return NotFound(new { message = "Product not found in Hash" });
        }
        
        return Ok(new { source = "Redis Hash", data = product });
    }

    [HttpGet("redis/hash")]
    public async Task<ActionResult<Dictionary<string, Product>>> GetAllProductsFromHash()
    {
        var hashKey = "products_hash";
        var products = await _redisCacheService.HashGetAllAsync<Product>(hashKey);
        
        return Ok(new { source = "Redis Hash", count = products.Count, data = products });
    }

    [HttpPost("redis/sortedset")]
    public async Task<ActionResult> AddProductToSortedSet([FromBody] Product product, [FromQuery] double score)
    {
        var sortedSetKey = "products_sortedset";
        await _redisCacheService.SortedSetAddAsync(sortedSetKey, product, score);
        
        return Ok(new { message = "Product added to Redis Sorted Set", key = sortedSetKey, score });
    }

    [HttpGet("redis/sortedset")]
    public async Task<ActionResult<List<Product>>> GetProductsSortedSet()
    {
        var sortedSetKey = "products_sortedset";
        var products = await _redisCacheService.SortedSetRangeAsync<Product>(sortedSetKey);
        
        return Ok(new { source = "Redis Sorted Set", count = products.Count, data = products });
    }

    [HttpDelete("cache/{id}")]
    public async Task<ActionResult> ClearCache(int id, [FromQuery] string cacheType = "all")
    {
        var tasks = new List<Task>();
        
        if (cacheType == "all" || cacheType == "memory")
        {
            tasks.Add(_memoryCacheService.RemoveAsync($"product_memory_{id}"));
        }
        
        if (cacheType == "all" || cacheType == "distributed")
        {
            tasks.Add(_distributedCacheService.RemoveAsync($"product_distributed_{id}"));
        }
        
        if (cacheType == "all" || cacheType == "redis")
        {
            tasks.Add(_redisCacheService.RemoveAsync($"product_redis_{id}"));
        }
        
        await Task.WhenAll(tasks);
        
        return Ok(new { message = $"Cache cleared for product {id}", cacheType });
    }

    private async Task<Product> SimulateDbQueryAsync(int id)
    {
        await Task.Delay(1000);
        
        return new Product
        {
            Id = id,
            Name = $"Product {id}",
            Description = $"This is a description for product {id}",
            Price = 99.99m * id,
            Category = id % 2 == 0 ? "Electronics" : "Clothing",
            StockQuantity = Random.Shared.Next(10, 100),
            CreatedAt = DateTime.UtcNow
        };
    }
}
