using LuxorLMS.Identity.Application.DTOs;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Enums;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using LuxorLMS.Kernel;

namespace LuxorLMS.Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDeviceSessionRepository _deviceSessionRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly int _refreshTokenExpiryDays;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IDeviceSessionRepository deviceSessionRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _deviceSessionRepository = deviceSessionRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
    }

    public async Task<Result<AuthResult>> RegisterAsync(string username, string email, string password, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Result<AuthResult>.Failure(new Error("USER_EXISTS", "User with this email already exists"));
        }

        if (await _userRepository.ExistsByUsernameAsync(username, cancellationToken))
        {
            return Result<AuthResult>.Failure(new Error("USERNAME_EXISTS", "Username is already taken"));
        }

        var passwordHash = _passwordHasher.Hash(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = string.Empty,
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(createdBy, "REGISTER", "User", user.Id.ToString(), null, user.Id.ToString(), cancellationToken: cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResult>.Success(new AuthResult(accessToken, refreshToken, user.Id, user.Username, user.Email, user.Role, user.MfaEnabled));
    }

    public async Task<Result<AuthResult>> LoginAsync(string usernameOrEmail, string password, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken)
            ?? await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);

        if (user == null)
        {
            await _auditLogger.LogAsync(Guid.Empty, "LOGIN_FAILED", "User", "unknown", null, "Invalid credentials", ipAddress, userAgent, cancellationToken);
            return Result<AuthResult>.Failure(new Error("INVALID_CREDENTIALS", "Invalid username or password"));
        }

        if (user.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            return Result<AuthResult>.Failure(new Error("ACCOUNT_LOCKED", "Account is locked. Please try again later."));
        }

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLocked = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogger.LogAsync(user.Id, "LOGIN_FAILED", "User", user.Id.ToString(), null, "Invalid password", ipAddress, userAgent, cancellationToken);
            return Result<AuthResult>.Failure(new Error("INVALID_CREDENTIALS", "Invalid username or password"));
        }

        if (user.IsLocked)
        {
            user.IsLocked = false;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
        }

        user.LastLogin = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        var deviceSession = new DeviceSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DeviceName = userAgent ?? "Unknown Device",
            UserAgent = userAgent ?? string.Empty,
            IpAddress = ipAddress ?? string.Empty,
            LastActiveAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _deviceSessionRepository.AddAsync(deviceSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(user.Id, "LOGIN", "User", user.Id.ToString(), null, "Success", ipAddress, userAgent, cancellationToken);

        return Result<AuthResult>.Success(new AuthResult(accessToken, refreshToken, user.Id, user.Username, user.Email, user.Role, user.MfaEnabled));
    }

    public async Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            return Result<AuthResult>.Failure(new Error("INVALID_REFRESH_TOKEN", "Refresh token is invalid or expired"));
        }

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId, cancellationToken);
        if (user == null)
        {
            return Result<AuthResult>.Failure(new Error("USER_NOT_FOUND", "User not found"));
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.ReplacedByToken = _jwtTokenService.GenerateRefreshToken();
        await _refreshTokenRepository.RevokeAsync(tokenEntity, cancellationToken);

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = tokenEntity.ReplacedByToken;

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow,
            ReplacedByToken = null,
            IpAddress = tokenEntity.IpAddress
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(user.Id, "REFRESH_TOKEN", "RefreshToken", tokenEntity.Id.ToString(), tokenEntity.Token, newRefreshToken, cancellationToken: cancellationToken);

        return Result<AuthResult>.Success(new AuthResult(newAccessToken, newRefreshToken, user.Id, user.Username, user.Email, user.Role, user.MfaEnabled));
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (tokenEntity == null)
        {
            return Result.Failure(new Error("TOKEN_NOT_FOUND", "Refresh token not found"));
        }

        await _refreshTokenRepository.RevokeAsync(tokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(tokenEntity.UserId, "REVOKE_TOKEN", "RefreshToken", tokenEntity.Id.ToString(), tokenEntity.Token, null, cancellationToken: cancellationToken);

        return Result.Success(default);
    }

    public async Task<Result> RevokeAllTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "REVOKE_ALL_TOKENS", "User", userId.ToString(), null, "All tokens revoked", cancellationToken: cancellationToken);

        return Result.Success(default);
    }
}
