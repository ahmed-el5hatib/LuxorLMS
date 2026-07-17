namespace LuxorLMS.Analytics.Domain.Entities;

public class ServerHealthMetric
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = "Healthy";
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
}
