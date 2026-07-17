using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Grading.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuxorLMS.Grading.Api.Filters;

public sealed class RequirePermissionFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthorizationService _authorizationService;
    private readonly string _permission;

    public RequirePermissionFilter(IAuthorizationService authorizationService, string permission)
    {
        _authorizationService = authorizationService;
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userId = PermissionHelper.GetCurrentUserId(context.HttpContext.User);
        if (userId is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var allowed = await _authorizationService.HasPermissionAsync(userId.Value, _permission, context.HttpContext.RequestAborted);
        if (!allowed)
        {
            context.Result = new ForbidResult();
        }
    }
}

public sealed class RequirePermissionFilterFactory : Attribute, IFilterFactory
{
    public string Permission { get; }

    public RequirePermissionFilterFactory(string permission)
    {
        Permission = permission;
    }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();
        return new RequirePermissionFilter(authService, Permission);
    }
}
