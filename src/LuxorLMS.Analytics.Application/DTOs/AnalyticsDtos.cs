namespace LuxorLMS.Analytics.Application.DTOs;

public record AnalyticsKpiDto(
    Guid Id,
    string Key,
    string Name,
    decimal Value,
    string? Unit,
    string MetricType,
    string TimeRange,
    Guid? CourseOfferingId,
    Guid? DepartmentId,
    Guid? ProgramId,
    DateTime CalculatedAt,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    string? Metadata
);

public record GpaTrendDto(
    Guid Id,
    Guid StudentId,
    Guid? CourseOfferingId,
    decimal SemesterGpa,
    decimal CumulativeGpa,
    int SemesterNumber,
    DateTime CalculatedAt
);

public record GradeDistributionDto(
    Guid Id,
    Guid CourseOfferingId,
    string GradeLetter,
    int StudentCount,
    decimal Percentage,
    decimal MinScore,
    decimal MaxScore,
    decimal AverageScore,
    DateTime CalculatedAt
);

public record ServerHealthMetricDto(
    Guid Id,
    string MetricName,
    decimal Value,
    string? Unit,
    string Status,
    DateTime RecordedAt,
    string? Details
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);

public record AnalyticsFilterRequest(
    Guid? CourseOfferingId,
    Guid? DepartmentId,
    Guid? ProgramId,
    string? MetricType,
    string? TimeRange,
    DateTime? From,
    DateTime? To,
    int PageNumber = 1,
    int PageSize = 50
);
