using LuxorLMS.Analytics.Domain.Enums;

namespace LuxorLMS.Analytics.Domain.Entities;

public class AnalyticsKpi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Unit { get; set; }
    public MetricType MetricType { get; set; }
    public TimeRange TimeRange { get; set; }
    public Guid? CourseOfferingId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ProgramId { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? Metadata { get; set; }
}
