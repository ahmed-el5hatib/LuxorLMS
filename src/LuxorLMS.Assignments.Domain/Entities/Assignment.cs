using LuxorLMS.Assignments.Domain.Enums;

namespace LuxorLMS.Assignments.Domain.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; }
    public bool AllowLateSubmission { get; set; } = false;
    public AssignmentStatus Status { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
