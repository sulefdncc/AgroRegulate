using AgroRegulate.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AgroRegulate.Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<bool> DecreaseQuotaAsync(string key, decimal amount)
    {
        var cachedValue = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedValue)) return true;

        if (decimal.TryParse(cachedValue, out decimal currentQuota))
        {
            if (currentQuota >= amount)
            {
                await _cache.SetStringAsync(key, (currentQuota - amount).ToString());
                return true;
            }
            return false;
        }

        return true;
    }

    public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task ReleaseLockAsync(string lockKey)
    {
        await Task.CompletedTask;
    }
}