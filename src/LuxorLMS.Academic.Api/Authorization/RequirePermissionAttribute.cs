using System.Security.Claims;

namespace LuxorLMS.Academic.Api.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    public string Permission { get; }

    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }
}

public static class PermissionHelper
{
    public static Guid? GetCurrentUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirst("sub")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
