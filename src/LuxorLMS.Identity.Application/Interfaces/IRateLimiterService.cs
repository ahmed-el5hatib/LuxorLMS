namespace LuxorLMS.Identity.Application.Interfaces;

public interface IRateLimiterService
{
    Task<bool> IsAllowedAsync(string key, int maxRequests, TimeSpan window, CancellationToken cancellationToken = default);
}
