namespace LuxorLMS.Storage.Domain.Entities;

public class FileVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StoredFileId { get; set; }
    public int Version { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string ContentHash { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoredFile? StoredFile { get; set; }
}
