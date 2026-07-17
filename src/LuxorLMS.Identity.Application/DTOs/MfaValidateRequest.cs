namespace LuxorLMS.Identity.Application.DTOs;

public record MfaValidateRequest(string MfaToken, string Code);
