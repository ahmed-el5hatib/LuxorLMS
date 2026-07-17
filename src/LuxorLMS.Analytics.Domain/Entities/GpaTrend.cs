namespace LuxorLMS.Analytics.Domain.Entities;

public class GpaTrend
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid? CourseOfferingId { get; set; }
    public decimal SemesterGpa { get; set; }
    public decimal CumulativeGpa { get; set; }
    public int SemesterNumber { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
