using LuxorLMS.Grading.Domain.Enums;

namespace LuxorLMS.Grading.Domain.Entities;

public class StudentGrade
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid SemesterId { get; set; }
    public int CreditHours { get; set; }
    public decimal RawScore { get; set; }
    public string GradeLetter { get; set; } = string.Empty;
    public decimal GradePoints { get; set; }
    public GradePublishStatus PublishStatus { get; set; } = GradePublishStatus.Draft;
    public Guid? DeptHeadApprovedBy { get; set; }
    public DateTime? DeptHeadApprovedAt { get; set; }
    public Guid? PublishedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? AppealDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
