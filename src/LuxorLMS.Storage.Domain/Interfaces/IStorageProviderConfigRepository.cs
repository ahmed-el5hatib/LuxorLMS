using LuxorLMS.Storage.Domain.Entities;

namespace LuxorLMS.Storage.Domain.Interfaces;

public interface IStorageProviderConfigRepository
{
    Task<StorageProviderConfig?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<StorageProviderConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StorageProviderConfig>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StorageProviderConfig config, CancellationToken cancellationToken = default);
    void Update(StorageProviderConfig config);
}
