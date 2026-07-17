using LuxorLMS.Storage.Domain.Enums;

namespace LuxorLMS.Storage.Domain.Entities;

public class StorageProviderConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public StorageProviderType ProviderType { get; set; }
    public string BucketOrContainer { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Endpoint { get; set; }
    public bool IsActive { get; set; }
}
