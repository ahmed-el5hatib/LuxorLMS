using LuxorLMS.Grading.Domain.Enums;

namespace LuxorLMS.Grading.Application.DTOs;

public record GradeAppealDto(
    Guid Id,
    Guid StudentGradeId,
    Guid StudentId,
    string Reason,
    AppealStatus Status,
    string? Resolution,
    Guid? ResolvedBy,
    DateTime? ResolvedAt,
    DateTime CreatedAt);

public record CreateGradeAppealRequest(
    Guid StudentGradeId,
    string Reason);

public record ResolveGradeAppealRequest(
    bool Approve,
    string? Resolution);
