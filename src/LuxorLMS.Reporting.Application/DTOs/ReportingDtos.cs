using LuxorLMS.Reporting.Domain.Enums;

namespace LuxorLMS.Reporting.Application.DTOs;

public record ReportJobDto(
    Guid Id,
    string ReportType,
    string Format,
    Guid RequestedBy,
    Guid? CourseOfferingId,
    Guid? StudentId,
    string Status,
    string? FilePath,
    string? FileName,
    long? FileSizeBytes,
    string? Error,
    DateTime RequestedAt,
    DateTime? CompletedAt
);

public record CreateReportRequest(
    ReportType ReportType,
    ExportFormat Format,
    Guid? CourseOfferingId,
    Guid? StudentId,
    string? Parameters
);

public record ReportTemplateDto(
    Guid Id,
    string Code,
    string Name,
    string ReportType,
    string Format,
    string TemplatePath,
    bool IsActive,
    DateTime CreatedAt
);
