namespace LuxorLMS.Analytics.Domain.Entities;

public class GradeDistribution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseOfferingId { get; set; }
    public string GradeLetter { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public decimal Percentage { get; set; }
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }
    public decimal AverageScore { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
