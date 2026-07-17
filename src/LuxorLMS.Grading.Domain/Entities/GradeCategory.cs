namespace LuxorLMS.Grading.Domain.Entities;

public class GradeCategory
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
