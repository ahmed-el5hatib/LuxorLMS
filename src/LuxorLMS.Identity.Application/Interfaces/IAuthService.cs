namespace LuxorLMS.Identity.Application.Interfaces;

using LuxorLMS.Kernel;

public interface IAuthService
{
    Task<Result<AuthResult>> RegisterAsync(string username, string email, string password, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<AuthResult>> LoginAsync(string usernameOrEmail, string password, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default);
    Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeAllTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}

public record AuthResult(string AccessToken, string RefreshToken, Guid UserId, string Username, string Email, LuxorLMS.Identity.Domain.Enums.UserRole Role, bool MfaRequired, string? MfaToken = null);
