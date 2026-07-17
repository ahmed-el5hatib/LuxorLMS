namespace LuxorLMS.Academic.Application.DTOs;

public record CourseDto(Guid Id, Guid DepartmentId, string CourseCode, string NameAr, string NameEn, int CreditHours, string? Description, bool IsActive, DateTime CreatedAt);
public record CreateCourseRequest(Guid DepartmentId, string CourseCode, string NameAr, string NameEn, int CreditHours, string? Description);
public record UpdateCourseRequest(string CourseCode, string NameAr, string NameEn, int CreditHours, string? Description, bool IsActive);
public record CoursePrerequisiteDto(Guid PrerequisiteCourseId, string CourseCode, string NameEn);
