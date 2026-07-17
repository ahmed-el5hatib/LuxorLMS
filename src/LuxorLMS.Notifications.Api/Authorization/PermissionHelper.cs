using System.Security.Claims;

namespace LuxorLMS.Notifications.Api.Authorization;

public static class PermissionHelper
{
    public static Guid? GetCurrentUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirst("sub")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
