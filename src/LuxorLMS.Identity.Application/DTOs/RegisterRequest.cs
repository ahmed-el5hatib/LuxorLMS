namespace LuxorLMS.Identity.Application.DTOs;

public record RegisterRequest(string Username, string Email, string Password, string FirstNameAr, string LastNameAr, string FirstNameEn, string LastNameEn, LuxorLMS.Identity.Domain.Enums.UserRole Role = LuxorLMS.Identity.Domain.Enums.UserRole.Student);
