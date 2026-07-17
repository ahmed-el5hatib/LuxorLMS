using LuxorLMS.Reporting.Domain.Enums;

namespace LuxorLMS.Reporting.Domain.Entities;

public class ReportTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public ExportFormat Format { get; set; }
    public string TemplatePath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
