using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Domain.Entities;

public class StoredFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public Guid? CourseOfferingId { get; set; }
    public string Container { get; set; } = string.Empty;
    public string ObjectKey { get; set; } = string.Empty;
    public StorageProviderType Provider { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int Version { get; set; } = 1;
    public string ContentHash { get; set; } = string.Empty;
    public bool IsCurrent { get; set; } = true;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<FileVersion> Versions { get; set; } = new();
}
