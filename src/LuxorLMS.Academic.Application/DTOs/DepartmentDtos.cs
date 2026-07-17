namespace LuxorLMS.Academic.Application.DTOs;

public record DepartmentDto(Guid Id, Guid FacultyId, string NameAr, string NameEn, string Code, Guid? HeadId, bool IsActive, DateTime CreatedAt);
public record CreateDepartmentRequest(Guid FacultyId, string NameAr, string NameEn, string Code, Guid? HeadId);
public record UpdateDepartmentRequest(string NameAr, string NameEn, string Code, Guid? HeadId, bool IsActive);
