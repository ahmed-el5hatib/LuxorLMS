using LuxorLMS.Grading.Domain.Enums;

namespace LuxorLMS.Grading.Domain.Entities;

public class GradeAppeal
{
    public Guid Id { get; set; }
    public Guid StudentGradeId { get; set; }
    public Guid StudentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AppealStatus Status { get; set; } = AppealStatus.Open;
    public string? Resolution { get; set; }
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
