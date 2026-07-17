namespace LuxorLMS.Academic.Application.DTOs;

public record StudyPlanDto(Guid Id, Guid ProgramId, string VersionCode, DateTime EffectiveFrom, DateTime? EffectiveTo, int MinimumCredits, bool IsActive, DateTime CreatedAt);
public record CreateStudyPlanRequest(Guid ProgramId, string VersionCode, DateTime EffectiveFrom, DateTime? EffectiveTo, int MinimumCredits);
public record UpdateStudyPlanRequest(string VersionCode, DateTime EffectiveFrom, DateTime? EffectiveTo, int MinimumCredits, bool IsActive);
