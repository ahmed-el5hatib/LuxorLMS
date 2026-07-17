namespace LuxorLMS.Grading.Domain.Entities;

public class GradeComponent
{
    public Guid Id { get; set; }
    public Guid GradeCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal MaxPoints { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
