using LuxorLMS.Identity.Application.DTOs;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Application.Services;
using LuxorLMS.Identity.Domain.Enums;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuthorizationService _authorizationService;

    public AuthController(IAuthService authService, IAuthorizationService authorizationService)
    {
        _authService = authService;
        _authorizationService = authorizationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password, Guid.Empty, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken, user = new { result.Value.UserId, result.Value.Username, result.Value.Email, role = result.Value.Role.ToString() } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.UsernameOrEmail, request.Password, cancellationToken: cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        if (result.Value.MfaRequired)
        {
            var mfaToken = GenerateMfaToken(result.Value.UserId);
            return Ok(new { mfaRequired = true, mfaToken, user = new { result.Value.UserId, result.Value.Username, result.Value.Email, role = result.Value.Role.ToString() } });
        }

        return Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken, user = new { result.Value.UserId, result.Value.Username, result.Value.Email, role = result.Value.Role.ToString() } });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return NoContent();
    }

    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _authService.RevokeAllTokensAsync(userId.Value, cancellationToken);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Code, description = result.Error.Description });

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _authorizationService.GetPermissionsAsync(userId.Value, cancellationToken);
        return Ok(new { userId = userId.Value, permissions = result });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private string GenerateMfaToken(Guid userId)
    {
        var token = $"mfa:{userId}:{Guid.NewGuid()}";
        // In production, store this in Redis with 5-minute expiry
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
    }
}
