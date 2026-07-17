namespace LuxorLMS.Academic.Application.DTOs;

public record SemesterDto(Guid Id, Guid AcademicYearId, string Name, string Code, DateTime StartDate, DateTime EndDate, DateTime RegistrationStart, DateTime RegistrationEnd, bool IsActive, DateTime CreatedAt);
public record CreateSemesterRequest(Guid AcademicYearId, string Name, string Code, DateTime StartDate, DateTime EndDate, DateTime RegistrationStart, DateTime RegistrationEnd);
public record UpdateSemesterRequest(string Name, string Code, DateTime StartDate, DateTime EndDate, DateTime RegistrationStart, DateTime RegistrationEnd, bool IsActive);
