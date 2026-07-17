namespace LuxorLMS.Registration.Application.DTOs;

public record RegistrationPeriodDto(Guid Id, Guid SemesterId, Guid? ProgramId, Guid AcademicYearId, DateTime StartDate, DateTime EndDate, DateTime? LateRegistrationStart, DateTime? LateRegistrationEnd, int MinCreditHours, int MaxCreditHours, decimal GpaCapForMax, bool IsActive, DateTime CreatedAt);
public record CreateRegistrationPeriodRequest(Guid SemesterId, Guid? ProgramId, Guid AcademicYearId, DateTime StartDate, DateTime EndDate, DateTime? LateRegistrationStart, DateTime? LateRegistrationEnd, int MinCreditHours, int MaxCreditHours, decimal GpaCapForMax);
public record UpdateRegistrationPeriodRequest(DateTime StartDate, DateTime EndDate, DateTime? LateRegistrationStart, DateTime? LateRegistrationEnd, int MinCreditHours, int MaxCreditHours, decimal GpaCapForMax, bool IsActive);
