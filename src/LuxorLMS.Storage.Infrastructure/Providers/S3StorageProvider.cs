using Amazon.S3;
using Amazon.S3.Model;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Infrastructure.Providers;

public class S3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _s3Client;

    public S3StorageProvider(IAmazonS3? s3Client = null)
    {
        _s3Client = s3Client ?? new AmazonS3Client();
    }

    public StorageProviderType ProviderType => StorageProviderType.S3;

    public async Task<string> UploadAsync(string container, string objectKey, Stream dataStream, string contentType, CancellationToken cancellationToken = default)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = container,
            Key = objectKey,
            InputStream = dataStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        return objectKey;
    }

    public Task<string> GetSignedUrlAsync(string container, string objectKey, TimeSpan expiresIn, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = container,
            Key = objectKey,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };

        var url = _s3Client.GetPreSignedURL(request);
        return Task.FromResult(url);
    }

    public async Task<Stream> DownloadAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = container,
            Key = objectKey
        };

        var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
        var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = container,
            Key = objectKey
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }
}
