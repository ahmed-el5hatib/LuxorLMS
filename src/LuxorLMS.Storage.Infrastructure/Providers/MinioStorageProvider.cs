using Minio;
using Minio.DataModel.Args;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Infrastructure.Providers;

public class MinioStorageProvider : IStorageProvider
{
    private readonly IMinioClient _minioClient;

    public MinioStorageProvider(IMinioClient? minioClient = null)
    {
        _minioClient = minioClient ?? new MinioClient().WithEndpoint("localhost:9000").WithCredentials("minioadmin", "minioadmin").Build();
    }

    public StorageProviderType ProviderType => StorageProviderType.MinIO;

    public async Task<string> UploadAsync(string container, string objectKey, Stream dataStream, string contentType, CancellationToken cancellationToken = default)
    {
        var beArgs = new BucketExistsArgs().WithBucket(container);
        bool found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(container);
            await _minioClient.MakeBucketAsync(mbArgs, cancellationToken);
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(container)
            .WithObject(objectKey)
            .WithStreamData(dataStream)
            .WithObjectSize(dataStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
        return objectKey;
    }

    public async Task<string> GetSignedUrlAsync(string container, string objectKey, TimeSpan expiresIn, CancellationToken cancellationToken = default)
    {
        var presignedArgs = new PresignedGetObjectArgs()
            .WithBucket(container)
            .WithObject(objectKey)
            .WithExpiry((int)expiresIn.TotalSeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedArgs);
    }

    public async Task<Stream> DownloadAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(container)
            .WithObject(objectKey)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteAsync(string container, string objectKey, CancellationToken cancellationToken = default)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(container)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }
}
