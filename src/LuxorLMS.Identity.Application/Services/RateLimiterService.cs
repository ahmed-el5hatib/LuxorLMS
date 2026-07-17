using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Infrastructure.Services;
using LuxorLMS.Kernel;

namespace LuxorLMS.Identity.Application.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IRedisConnectionFactory _redis;

    public RateLimiterService(IRedisConnectionFactory redis)
    {
        _redis = redis;
    }

    public async Task<bool> IsAllowedAsync(string key, int maxRequests, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var windowStart = now - window.TotalMilliseconds;

        await db.KeyExpireAsync(key, window);
        var count = await db.SortedSetLengthAsync(key, windowStart, now);

        if (count >= maxRequests)
        {
            return false;
        }

        await db.SortedSetAddAsync(key, now.ToString(), now);
        return true;
    }
}
