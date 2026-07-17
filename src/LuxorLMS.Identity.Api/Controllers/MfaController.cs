using LuxorLMS.Identity.Application.DTOs;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MfaController : ControllerBase
{
    private readonly IMfaAppService _mfaAppService;

    public MfaController(IMfaAppService mfaAppService)
    {
        _mfaAppService = mfaAppService;
    }

    [HttpPost("enable")]
    public async Task<IActionResult> Enable(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var secretResult = await _mfaAppService.EnableMfaAsync(userId.Value, cancellationToken);
        if (secretResult.IsFailure) return BadRequest(new { error = secretResult.Error.Code, description = secretResult.Error.Description });

        // In production, generate QR code from the URI
        var qrCodeUri = GenerateQrCodeUri(User.Identity?.Name ?? "user", secretResult.Value);
        return Ok(new MfaEnableResponse(secretResult.Value, qrCodeUri));
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] MfaValidateRequest request, CancellationToken cancellationToken)
    {
        var result = await _mfaAppService.ValidateMfaCodeAsync(request.MfaToken, request.Code, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return Ok(new { message = "MFA code validated successfully" });
    }

    [HttpPost("disable")]
    public async Task<IActionResult> Disable([FromBody] string password, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _mfaAppService.DisableMfaAsync(userId.Value, password, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return Ok(new { message = "MFA disabled successfully" });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private string GenerateQrCodeUri(string email, string secret)
    {
        return $"otpauth://totp/LuxorLMS:{Uri.EscapeDataString(email)}?secret={secret}&issuer=LuxorLMS&digits=6";
    }
}
