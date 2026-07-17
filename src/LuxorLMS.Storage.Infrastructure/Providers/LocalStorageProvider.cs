using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Infrastructure.Providers;

public class LocalStorageProvider : IStorageProvider
{
    private readonly string _basePath;

    public LocalStorageProvider(string? basePath = null)
    {
        _basePath = basePath ?? Path.Combine(Path.GetTempPath(), "LuxorLMS_Storage");
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public StorageProviderType ProviderType => StorageProviderType.LocalStorage;

    public async Task<string> UploadAsync(string container, string objectKey, Stream dataStream, string contentType, CancellationToken cancellationToken = default)
    {
        var dir = Path.Combine(_basePath, container);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var filePath = Path.Combine(dir, objectKey);
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await dataStream.CopyToAsync(fileStream, cancellationToken);

        return objectKey;
    }

    public Task<string> GetSignedUrlAsync(string container, string objectKey, TimeSpan expiresIn, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, container, objectKey);
        var fileUri = new Uri(filePath).AbsoluteUri;
        return Task.FromResult($"{fileUri}?exp={DateTimeOffset.UtcNow.Add(expiresIn).ToUnixTimeSeconds()}");
    }

    public Task<Stream> DownloadAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, container, objectKey);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found at path {filePath}");
        }

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, container, objectKey);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
