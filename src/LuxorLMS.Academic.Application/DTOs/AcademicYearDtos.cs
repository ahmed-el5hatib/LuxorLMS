namespace LuxorLMS.Academic.Application.DTOs;

public record AcademicYearDto(Guid Id, string Label, DateTime StartDate, DateTime EndDate, bool IsActive, DateTime CreatedAt);
public record CreateAcademicYearRequest(string Label, DateTime StartDate, DateTime EndDate);
