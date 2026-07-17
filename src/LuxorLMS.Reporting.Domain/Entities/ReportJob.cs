using LuxorLMS.Reporting.Domain.Enums;

namespace LuxorLMS.Reporting.Domain.Entities;

public class ReportJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ReportType ReportType { get; set; }
    public ExportFormat Format { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? CourseOfferingId { get; set; }
    public Guid? StudentId { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Queued;
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? Error { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Parameters { get; set; }
}
