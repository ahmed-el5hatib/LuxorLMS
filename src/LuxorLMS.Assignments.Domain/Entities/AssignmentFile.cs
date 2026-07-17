namespace LuxorLMS.Assignments.Domain.Entities;

public class AssignmentFile
{
    public Guid Id { get; set; }
    public Guid AssignmentSubmissionId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public int Version { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
