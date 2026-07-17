using LuxorLMS.Administration.Domain.Enums;

namespace LuxorLMS.Administration.Domain.Entities;

public class SystemSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public SettingType Type { get; set; } = SettingType.String;
    public string? Description { get; set; }
    public bool IsSensitive { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
