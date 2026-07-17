namespace LuxorLMS.Identity.Application.DTOs;

public record MfaEnableResponse(string Secret, string QrCodeUri);
