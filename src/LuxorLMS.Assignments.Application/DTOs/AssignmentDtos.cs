using LuxorLMS.Assignments.Domain.Enums;

namespace LuxorLMS.Assignments.Application.DTOs;

public record AssignmentDto(
    Guid Id,
    Guid CourseOfferingId,
    string Title,
    string? Description,
    DateTime DueDate,
    decimal MaxScore,
    bool AllowLateSubmission,
    AssignmentStatus Status,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record CreateAssignmentRequest(
    Guid CourseOfferingId,
    string Title,
    string? Description,
    DateTime DueDate,
    decimal MaxScore,
    bool AllowLateSubmission);

public record UpdateAssignmentRequest(
    string Title,
    string? Description,
    DateTime DueDate,
    decimal MaxScore,
    bool AllowLateSubmission);

public record AssignmentRubricDto(
    Guid Id,
    Guid AssignmentId,
    string Criteria,
    decimal MaxPoints,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record CreateAssignmentRubricRequest(
    Guid AssignmentId,
    string Criteria,
    decimal MaxPoints,
    string? Description,
    int DisplayOrder);

public record UpdateAssignmentRubricRequest(
    string Criteria,
    decimal MaxPoints,
    string? Description,
    int DisplayOrder);

public record AssignmentSubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    DateTime SubmittedAt,
    decimal? Score,
    string? Feedback,
    SubmissionStatus Status,
    string? PlagiarismReportUrl,
    decimal? PlagiarismScore,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record SubmitAssignmentRequest(
    Guid AssignmentId,
    Guid StudentId);

public record GradeSubmissionRequest(
    decimal? Score,
    string? Feedback);

public record ReturnSubmissionRequest(
    string? Feedback);

public record AssignmentFileDto(
    Guid Id,
    Guid AssignmentSubmissionId,
    string FileName,
    string FileUrl,
    string? ContentType,
    long FileSizeBytes,
    int Version,
    DateTime UploadedAt,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record AddAssignmentFileRequest(
    Guid AssignmentSubmissionId,
    string FileName,
    string FileUrl,
    string? ContentType,
    long FileSizeBytes);

public record PlagiarismReportDto(
    Guid SubmissionId,
    decimal? PlagiarismScore,
    string? ReportUrl);
