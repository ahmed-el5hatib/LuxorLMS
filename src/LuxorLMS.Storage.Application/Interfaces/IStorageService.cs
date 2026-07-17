using LuxorLMS.Kernel;
using LuxorLMS.Storage.Application.DTOs;

namespace LuxorLMS.Storage.Application.Interfaces;

public interface IStorageService
{
    Task<Result<StoredFileDto>> UploadAsync(UploadFileRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<StoredFileDto>> UploadNewVersionAsync(Guid storedFileId, Stream dataStream, long sizeBytes, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<string>> GetSignedUrlAsync(Guid storedFileId, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<FileVersionDto>>> GetVersionsAsync(Guid storedFileId, CancellationToken cancellationToken = default);
    Task<Result<StoredFileDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid storedFileId, CancellationToken cancellationToken = default);
}
