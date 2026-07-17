namespace LuxorLMS.Identity.Application.DTOs;

public record UserProfileResponse(Guid Id, string Username, string Email, string FirstNameAr, string LastNameAr, string FirstNameEn, string LastNameEn, LuxorLMS.Identity.Domain.Enums.UserRole Role, bool MfaEnabled, DateTime CreatedAt, DateTime? LastLogin);
