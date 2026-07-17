namespace LuxorLMS.Registration.Application.Concurrency;

public interface ISeatBookingLock : IAsyncDisposable
{
    bool IsAcquired { get; }
}

public interface ISeatBookingLockFactory
{
    Task<ISeatBookingLock> AcquireAsync(string resource, TimeSpan expiry, CancellationToken cancellationToken = default);
}
