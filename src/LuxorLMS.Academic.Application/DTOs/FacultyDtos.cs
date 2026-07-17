namespace LuxorLMS.Academic.Application.DTOs;

public record FacultyDto(Guid Id, string NameAr, string NameEn, string Code, bool IsActive, DateTime CreatedAt);
public record CreateFacultyRequest(string NameAr, string NameEn, string Code);
public record UpdateFacultyRequest(string NameAr, string NameEn, string Code, bool IsActive);
