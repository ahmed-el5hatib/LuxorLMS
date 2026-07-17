namespace LuxorLMS.Identity.Application.Interfaces;

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
    Task<bool> HasRoleAsync(Guid userId, LuxorLMS.Identity.Domain.Enums.UserRole role, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
