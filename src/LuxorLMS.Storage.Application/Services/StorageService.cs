using System.Security.Cryptography;
using LuxorLMS.Kernel;
using LuxorLMS.Storage.Application.DTOs;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Domain.Entities;
using LuxorLMS.Storage.Domain.Enums;
using LuxorLMS.Storage.Domain.Interfaces;

namespace LuxorLMS.Storage.Application.Services;

public class StorageService : IStorageService
{
    private readonly IStoredFileRepository _fileRepository;
    private readonly IFileVersionRepository _versionRepository;
    private readonly IStorageProviderConfigRepository _configRepository;
    private readonly IEnumerable<IStorageProvider> _providers;
    private readonly IStorageUnitOfWork _unitOfWork;

    public StorageService(
        IStoredFileRepository fileRepository,
        IFileVersionRepository versionRepository,
        IStorageProviderConfigRepository configRepository,
        IEnumerable<IStorageProvider> providers,
        IStorageUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _versionRepository = versionRepository;
        _configRepository = configRepository;
        _providers = providers;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoredFileDto>> UploadAsync(UploadFileRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var activeConfig = await _configRepository.GetActiveAsync(cancellationToken);
        var providerType = activeConfig?.ProviderType ?? StorageProviderType.LocalStorage;
        var container = !string.IsNullOrWhiteSpace(request.Container)
            ? request.Container
            : activeConfig?.BucketOrContainer ?? "luxorlms-files";

        var provider = ResolveProvider(providerType);
        var objectKey = $"{Guid.NewGuid()}_{request.FileName}";

        var hash = await ComputeHashAsync(request.DataStream, cancellationToken);
        request.DataStream.Position = 0;

        await provider.UploadAsync(container, objectKey, request.DataStream, request.ContentType, cancellationToken);

        var storedFile = new StoredFile
        {
            OwnerId = request.OwnerId,
            CourseOfferingId = request.CourseOfferingId,
            Container = container,
            ObjectKey = objectKey,
            Provider = providerType,
            FileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            Version = 1,
            ContentHash = hash,
            IsCurrent = true,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _fileRepository.AddAsync(storedFile, cancellationToken);

        var fileVersion = new FileVersion
        {
            StoredFileId = storedFile.Id,
            Version = 1,
            ObjectKey = objectKey,
            SizeBytes = request.SizeBytes,
            ContentHash = hash,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _versionRepository.AddAsync(fileVersion, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StoredFileDto>.Success(MapToDto(storedFile));
    }

    public async Task<Result<StoredFileDto>> UploadNewVersionAsync(Guid storedFileId, Stream dataStream, long sizeBytes, Guid userId, CancellationToken cancellationToken = default)
    {
        var storedFile = await _fileRepository.GetByIdAsync(storedFileId, cancellationToken);
        if (storedFile is null)
            return Result<StoredFileDto>.Failure(new Error("Storage.NotFound", "File not found."));

        var provider = ResolveProvider(storedFile.Provider);
        var newVersionNumber = storedFile.Version + 1;
        var objectKey = $"{Guid.NewGuid()}_v{newVersionNumber}_{storedFile.FileName}";

        var hash = await ComputeHashAsync(dataStream, cancellationToken);
        dataStream.Position = 0;

        await provider.UploadAsync(storedFile.Container, objectKey, dataStream, storedFile.ContentType, cancellationToken);

        var oldVersionRecord = new FileVersion
        {
            StoredFileId = storedFile.Id,
            Version = storedFile.Version,
            ObjectKey = storedFile.ObjectKey,
            SizeBytes = storedFile.SizeBytes,
            ContentHash = storedFile.ContentHash,
            CreatedBy = storedFile.CreatedBy,
            CreatedAt = storedFile.CreatedAt
        };
        await _versionRepository.AddAsync(oldVersionRecord, cancellationToken);

        storedFile.Version = newVersionNumber;
        storedFile.ObjectKey = objectKey;
        storedFile.SizeBytes = sizeBytes;
        storedFile.ContentHash = hash;
        storedFile.IsCurrent = true;
        _fileRepository.Update(storedFile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StoredFileDto>.Success(MapToDto(storedFile));
    }

    public async Task<Result<string>> GetSignedUrlAsync(Guid storedFileId, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        var storedFile = await _fileRepository.GetByIdAsync(storedFileId, cancellationToken);
        if (storedFile is null)
            return Result<string>.Failure(new Error("Storage.NotFound", "File not found."));

        var provider = ResolveProvider(storedFile.Provider);
        var duration = expiresIn ?? TimeSpan.FromMinutes(15);
        var signedUrl = await provider.GetSignedUrlAsync(storedFile.Container, storedFile.ObjectKey, duration, cancellationToken);

        return Result<string>.Success(signedUrl);
    }

    public async Task<Result<IReadOnlyList<FileVersionDto>>> GetVersionsAsync(Guid storedFileId, CancellationToken cancellationToken = default)
    {
        var storedFile = await _fileRepository.GetByIdAsync(storedFileId, cancellationToken);
        if (storedFile is null)
            return Result<IReadOnlyList<FileVersionDto>>.Failure(new Error("Storage.NotFound", "File not found."));

        var versions = await _versionRepository.GetByStoredFileIdAsync(storedFileId, cancellationToken);
        var dtos = versions.Select(v => new FileVersionDto(v.Id, v.StoredFileId, v.Version, v.ObjectKey, v.SizeBytes, v.ContentHash, v.CreatedBy, v.CreatedAt)).ToList();

        return Result<IReadOnlyList<FileVersionDto>>.Success(dtos);
    }

    public async Task<Result<StoredFileDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var storedFile = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (storedFile is null)
            return Result<StoredFileDto>.Failure(new Error("Storage.NotFound", "File not found."));

        return Result<StoredFileDto>.Success(MapToDto(storedFile));
    }

    public async Task<Result> DeleteAsync(Guid storedFileId, CancellationToken cancellationToken = default)
    {
        var storedFile = await _fileRepository.GetByIdAsync(storedFileId, cancellationToken);
        if (storedFile is null)
            return Result.Failure(new Error("Storage.NotFound", "File not found."));

        var provider = ResolveProvider(storedFile.Provider);
        await provider.DeleteAsync(storedFile.Container, storedFile.ObjectKey, cancellationToken);

        _fileRepository.Delete(storedFile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private IStorageProvider ResolveProvider(StorageProviderType providerType)
    {
        var provider = _providers.FirstOrDefault(p => p.ProviderType == providerType);
        if (provider is null)
        {
            // Fallback to LocalStorage
            provider = _providers.FirstOrDefault(p => p.ProviderType == StorageProviderType.LocalStorage);
        }
        return provider ?? throw new InvalidOperationException($"No storage provider registered for {providerType}.");
    }

    private static async Task<string> ComputeHashAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hashBytes);
    }

    private static StoredFileDto MapToDto(StoredFile file) => new(
        file.Id,
        file.OwnerId,
        file.CourseOfferingId,
        file.Container,
        file.ObjectKey,
        file.Provider,
        file.FileName,
        file.ContentType,
        file.SizeBytes,
        file.Version,
        file.ContentHash,
        file.IsCurrent,
        file.CreatedBy,
        file.CreatedAt
    );
}
