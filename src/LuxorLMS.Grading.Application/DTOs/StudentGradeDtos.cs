using LuxorLMS.Grading.Domain.Enums;

namespace LuxorLMS.Grading.Application.DTOs;

public record StudentGradeDto(
    Guid Id,
    Guid CourseOfferingId,
    Guid StudentId,
    Guid CourseId,
    Guid SemesterId,
    int CreditHours,
    decimal RawScore,
    string GradeLetter,
    decimal GradePoints,
    GradePublishStatus PublishStatus,
    Guid? DeptHeadApprovedBy,
    DateTime? DeptHeadApprovedAt,
    Guid? PublishedBy,
    DateTime? PublishedAt,
    DateTime? AppealDeadline,
    DateTime CreatedAt);

public record EnterGradeRequest(
    Guid CourseOfferingId,
    Guid StudentId,
    Guid CourseId,
    Guid SemesterId,
    decimal RawScore);

public record UpdateGradeRequest(decimal RawScore);
