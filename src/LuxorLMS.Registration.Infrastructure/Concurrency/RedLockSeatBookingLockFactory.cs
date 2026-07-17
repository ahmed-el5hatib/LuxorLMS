using LuxorLMS.Registration.Application.Concurrency;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace LuxorLMS.Registration.Infrastructure.Concurrency;

public sealed class NoOpSeatBookingLock : ISeatBookingLock
{
    public bool IsAcquired => true;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class RedLockSeatBookingLock : ISeatBookingLock
{
    private readonly IRedLock _instance;

    public RedLockSeatBookingLock(IRedLock instance)
    {
        _instance = instance;
    }

    public bool IsAcquired => _instance.IsAcquired;

    public async ValueTask DisposeAsync()
    {
        if (_instance is IAsyncDisposable ad)
            await ad.DisposeAsync();
        else
            _instance.Dispose();
    }
}

public class RedLockSeatBookingLockFactory : ISeatBookingLockFactory
{
    private readonly string _redisConnectionString;

    public RedLockSeatBookingLockFactory(string redisConnectionString)
    {
        _redisConnectionString = redisConnectionString;
    }

    public async Task<ISeatBookingLock> AcquireAsync(string resource, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoints = _redisConnectionString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var redisEndpoints = endpoints.Select(e =>
            {
                var uri = e.StartsWith("redis://", StringComparison.OrdinalIgnoreCase) ? e : "redis://" + e;
                var u = new Uri(uri);
                return new RedLockEndPoint { EndPoint = new System.Net.DnsEndPoint(u.Host, u.Port) };
            }).ToList();

            using var factory = RedLockFactory.Create(redisEndpoints);
            var instance = await factory.CreateLockAsync(resource, expiry, expiry, TimeSpan.FromMilliseconds(50), cancellationToken);

            if (instance.IsAcquired)
                return new RedLockSeatBookingLock(instance);

            instance.Dispose();
            return new NoOpSeatBookingLock();
        }
        catch
        {
            // Redis unavailable: degrade to no-op to keep registration available (best-effort).
            return new NoOpSeatBookingLock();
        }
    }
}
