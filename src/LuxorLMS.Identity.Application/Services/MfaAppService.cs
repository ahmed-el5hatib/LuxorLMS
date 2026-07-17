using LuxorLMS.Identity.Application.DTOs;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Domain.Enums;
using LuxorLMS.Identity.Infrastructure.Services;
using LuxorLMS.Kernel;

namespace LuxorLMS.Identity.Application.Services;

public class MfaAppService : IMfaAppService
{
    private readonly IUserRepository _userRepository;
    private readonly IMfaService _mfaService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly TimeSpan _mfaTokenExpiry = TimeSpan.FromMinutes(5);

    public MfaAppService(
        IUserRepository userRepository,
        IMfaService mfaService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger)
    {
        _userRepository = userRepository;
        _mfaService = mfaService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<Result<string>> EnableMfaAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure(new Error("USER_NOT_FOUND", "User not found"));
        }

        var secret = _mfaService.GenerateSecret();
        user.MfaSecret = secret;
        user.MfaEnabled = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "ENABLE_MFA", "User", user.Id.ToString(), null, "MFA enabled", cancellationToken: cancellationToken);

        return Result<string>.Success(secret);
    }

    public async Task<Result> VerifyMfaAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret))
        {
            return Result.Failure(new Error("MFA_NOT_SETUP", "MFA is not set up for this user"));
        }

        var isValid = await _mfaService.ValidateAsync(user.MfaSecret, code);
        if (!isValid)
        {
            return Result.Failure(new Error("INVALID_MFA_CODE", "Invalid MFA code"));
        }

        await _auditLogger.LogAsync(userId, "VERIFY_MFA", "User", user.Id.ToString(), null, "MFA verified", cancellationToken: cancellationToken);

        return Result.Success(default);
    }

    public async Task<Result> DisableMfaAsync(Guid userId, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(new Error("USER_NOT_FOUND", "User not found"));
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return Result.Failure(new Error("INVALID_PASSWORD", "Invalid password"));
        }

        user.MfaEnabled = false;
        user.MfaSecret = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "DISABLE_MFA", "User", user.Id.ToString(), "MFA enabled", "MFA disabled", cancellationToken: cancellationToken);

        return Result.Success(default);
    }

    public Task<Result> ValidateMfaCodeAsync(string mfaToken, string code, CancellationToken cancellationToken = default)
    {
        var principal = _jwtTokenService.ValidateAccessToken(mfaToken);
        if (principal == null)
        {
            return Task.FromResult<Result>(Result.Failure(new Error("INVALID_MFA_TOKEN", "MFA token is invalid or expired")));
        }

        var userIdClaim = principal.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Task.FromResult<Result>(Result.Failure(new Error("INVALID_TOKEN", "Invalid token payload")));
        }

        return VerifyMfaAsync(userId, code, cancellationToken);
    }
}
