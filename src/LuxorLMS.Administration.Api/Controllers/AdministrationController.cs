using LuxorLMS.Administration.Api.Filters;
using LuxorLMS.Administration.Application.DTOs;
using LuxorLMS.Administration.Application.Interfaces;
using LuxorLMS.Administration.Application.Permissions;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Administration.Api.Controllers;

[ApiController]
[Route("api/v1/admin/settings")]
public class SystemSettingsController : ControllerBase
{
    private readonly IAdministrationService _administrationService;

    public SystemSettingsController(IAdministrationService administrationService)
    {
        _administrationService = administrationService;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AdministrationPermissions.ManageSettings)]
    public async Task<IActionResult> GetSettings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await _administrationService.GetSettingsAsync(pageNumber, pageSize, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{key}")]
    [RequirePermissionFilterFactory(AdministrationPermissions.ManageSettings)]
    public async Task<IActionResult> GetSetting(string key, CancellationToken cancellationToken)
    {
        var result = await _administrationService.GetSettingByKeyAsync(key, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPut("{key}")]
    [RequirePermissionFilterFactory(AdministrationPermissions.ManageSettings)]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _administrationService.UpdateSettingAsync(key, request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("seed")]
    [RequirePermissionFilterFactory(AdministrationPermissions.ManageSettings)]
    public async Task<IActionResult> SeedSettings(CancellationToken cancellationToken)
    {
        var result = await _administrationService.SeedSettingsAsync(cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(new { message = "Settings seeded successfully." });
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

[ApiController]
[Route("api/v1/admin/logs")]
public class SystemLogsController : ControllerBase
{
    private readonly IAdministrationService _administrationService;

    public SystemLogsController(IAdministrationService administrationService)
    {
        _administrationService = administrationService;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AdministrationPermissions.ViewLogs)]
    public async Task<IActionResult> GetLogs([FromQuery] LogsFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _administrationService.GetLogsAsync(filter, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

[ApiController]
[Route("api/v1/admin/jobs")]
public class BackgroundJobsController : ControllerBase
{
    private readonly IAdministrationService _administrationService;

    public BackgroundJobsController(IAdministrationService administrationService)
    {
        _administrationService = administrationService;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AdministrationPermissions.ManageBackgroundJobs)]
    public async Task<IActionResult> GetBackgroundJobs(CancellationToken cancellationToken)
    {
        var result = await _administrationService.GetBackgroundJobsAsync(cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}
