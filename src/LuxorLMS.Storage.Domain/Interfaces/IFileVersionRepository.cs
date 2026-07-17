using LuxorLMS.Storage.Domain.Entities;

namespace LuxorLMS.Storage.Domain.Interfaces;

public interface IFileVersionRepository
{
    Task<IReadOnlyList<FileVersion>> GetByStoredFileIdAsync(Guid storedFileId, CancellationToken cancellationToken = default);
    Task AddAsync(FileVersion version, CancellationToken cancellationToken = default);
}
