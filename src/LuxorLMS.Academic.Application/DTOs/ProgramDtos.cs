using LuxorLMS.Academic.Domain.Enums;

namespace LuxorLMS.Academic.Application.DTOs;

public record ProgramDto(Guid Id, Guid DepartmentId, string NameAr, string NameEn, string Code, DegreeLevel DegreeLevel, int TotalCreditsRequired, bool IsActive, DateTime CreatedAt);
public record CreateProgramRequest(Guid DepartmentId, string NameAr, string NameEn, string Code, DegreeLevel DegreeLevel, int TotalCreditsRequired);
public record UpdateProgramRequest(string NameAr, string NameEn, string Code, DegreeLevel DegreeLevel, int TotalCreditsRequired, bool IsActive);
