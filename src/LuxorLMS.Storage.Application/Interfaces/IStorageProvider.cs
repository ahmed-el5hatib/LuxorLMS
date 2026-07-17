using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Application.Interfaces;

public interface IStorageProvider
{
    StorageProviderType ProviderType { get; }
    Task<string> UploadAsync(string container, string objectKey, Stream dataStream, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetSignedUrlAsync(string container, string objectKey, TimeSpan expiresIn, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string container, string objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string container, string objectKey, CancellationToken cancellationToken = default);
}
