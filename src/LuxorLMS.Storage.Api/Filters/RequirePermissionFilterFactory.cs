using LuxorLMS.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuxorLMS.Storage.Api.Filters;

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
