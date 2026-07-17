using LuxorLMS.Storage.Domain.Entities;

namespace LuxorLMS.Storage.Domain.Interfaces;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StoredFile>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(StoredFile storedFile, CancellationToken cancellationToken = default);
    void Update(StoredFile storedFile);
    void Delete(StoredFile storedFile);
}
