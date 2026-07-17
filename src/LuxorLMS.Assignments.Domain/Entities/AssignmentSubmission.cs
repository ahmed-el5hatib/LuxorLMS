using LuxorLMS.Assignments.Domain.Enums;

namespace LuxorLMS.Assignments.Domain.Entities;

public class AssignmentSubmission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public SubmissionStatus Status { get; set; }
    public string? PlagiarismReportUrl { get; set; }
    public decimal? PlagiarismScore { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
