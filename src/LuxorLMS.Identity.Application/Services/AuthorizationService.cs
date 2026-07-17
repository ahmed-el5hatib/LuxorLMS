using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Domain.Enums;
using LuxorLMS.Kernel;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LuxorLMS.Identity.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly ConcurrentDictionary<string, (List<string> Permissions, DateTime Expiry)> _cache = new();
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    private static readonly Dictionary<UserRole, string[]> RolePermissions = new()
    {
        [UserRole.SystemAdmin] = new[] { "*" },
        [UserRole.AcademicAffairs] = new[] { "registration:manage", "grade:publish", "calendar:manage", "transcript:audit" },
        [UserRole.QAOfficer] = new[] { "course:read", "grade:read", "attendance:read", "report:read" },
        [UserRole.FacultyManager] = new[] { "faculty:settings", "staff:allocate", "quota:manage", "analytics:view" },
        [UserRole.DepartmentManager] = new[] { "program:manage", "studyplan:manage", "staff:assign", "grade:review" },
        [UserRole.Doctor] = new[] { "course:create", "quiz:create", "assignment:create", "grade:manage", "attendance:qr", "forum:moderate" },
        [UserRole.TeachingAssistant] = new[] { "attendance:mark", "grade:restricted", "forum:moderate", "lab:lead" },
        [UserRole.Student] = new[] { "course:register", "material:download", "assignment:submit", "quiz:take", "grade:view", "transcript:view" }
    };

    public AuthorizationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        var permissions = await GetPermissionsAsync(userId, cancellationToken);
        return permissions.Contains("*") || permissions.Contains(permission);
    }

    public async Task<bool> HasRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user != null && user.Role == role;
    }

    public async Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"permissions:{userId}";
        if (_cache.TryGetValue(cacheKey, out var cached) && cached.Expiry > DateTime.UtcNow)
        {
            return cached.Permissions;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return Enumerable.Empty<string>();

        var permissions = RolePermissions.TryGetValue(user.Role, out var perms) ? perms.ToList() : new List<string>();
        _cache[cacheKey] = (permissions, DateTime.UtcNow.Add(_cacheDuration));

        return permissions;
    }
}
