using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Infrastructure.Providers;

public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageProvider(BlobServiceClient? blobServiceClient = null)
    {
        _blobServiceClient = blobServiceClient ?? new BlobServiceClient("UseDevelopmentStorage=true");
    }

    public StorageProviderType ProviderType => StorageProviderType.AzureBlob;

    public async Task<string> UploadAsync(string container, string objectKey, Stream dataStream, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(objectKey);
        await blobClient.UploadAsync(dataStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return objectKey;
    }

    public Task<string> GetSignedUrlAsync(string container, string objectKey, TimeSpan expiresIn, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(objectKey);

        if (blobClient.CanGenerateSasUri)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container,
                BlobName = objectKey,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return Task.FromResult(sasUri.ToString());
        }

        return Task.FromResult(blobClient.Uri.ToString());
    }

    public async Task<Stream> DownloadAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(objectKey);

        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(objectKey);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
