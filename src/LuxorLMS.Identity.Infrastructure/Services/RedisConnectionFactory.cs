using StackExchange.Redis;

namespace LuxorLMS.Identity.Infrastructure.Services;

public interface IRedisConnectionFactory
{
    Task<ConnectionMultiplexer> GetConnectionAsync(CancellationToken cancellationToken = default);
    IDatabase GetDatabase(int db = -1);
    IServer GetServer(string hostAndPort);
}

public class RedisConnectionFactory : IRedisConnectionFactory, IDisposable
{
    private readonly string _connectionString;
    private Lazy<ConnectionMultiplexer>? _lazyConnection;

    public RedisConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionString));
    }

    public async Task<ConnectionMultiplexer> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_lazyConnection == null)
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionString));
        }

        return await Task.FromResult(_lazyConnection.Value);
    }

    public IDatabase GetDatabase(int db = -1)
    {
        return _lazyConnection?.Value.GetDatabase(db) ?? throw new InvalidOperationException("Redis connection is not initialized");
    }

    public IServer GetServer(string hostAndPort)
    {
        return _lazyConnection?.Value.GetServer(hostAndPort) ?? throw new InvalidOperationException("Redis connection is not initialized");
    }

    public void Dispose()
    {
        _lazyConnection?.Value?.Dispose();
        _lazyConnection = null;
    }
}
