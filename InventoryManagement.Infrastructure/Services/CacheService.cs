using InventoryManagement.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryManagement.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        if (_cache.TryGetValue(key, out T? value) && value is not null)
            return value;

        value = await factory();
        _cache.Set(key, value, TimeSpan.FromMinutes(5));

        return value!;
    }
}