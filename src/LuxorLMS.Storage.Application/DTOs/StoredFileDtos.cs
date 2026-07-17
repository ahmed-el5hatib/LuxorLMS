using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Application.DTOs;

public record StoredFileDto(
    Guid Id,
    Guid OwnerId,
    Guid? CourseOfferingId,
    string Container,
    string ObjectKey,
    StorageProviderType Provider,
    string FileName,
    string ContentType,
    long SizeBytes,
    int Version,
    string ContentHash,
    bool IsCurrent,
    Guid CreatedBy,
    DateTime CreatedAt
);

public record FileVersionDto(
    Guid Id,
    Guid StoredFileId,
    int Version,
    string ObjectKey,
    long SizeBytes,
    string ContentHash,
    Guid CreatedBy,
    DateTime CreatedAt
);

public record StorageProviderConfigDto(
    Guid Id,
    StorageProviderType ProviderType,
    string BucketOrContainer,
    string Region,
    string? Endpoint,
    bool IsActive
);

public record UploadFileRequest(
    Guid OwnerId,
    Guid? CourseOfferingId,
    string FileName,
    string ContentType,
    Stream DataStream,
    long SizeBytes,
    string? Container = null
);
