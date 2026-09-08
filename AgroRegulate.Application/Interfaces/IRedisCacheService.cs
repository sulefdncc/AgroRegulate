namespace AgroRegulate.Application.Interfaces;

public interface IRedisCacheService
{
    Task<bool> DecreaseQuotaAsync(string key, decimal amount);
    Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry);
    Task ReleaseLockAsync(string lockKey);
}