using LuxorLMS.Reporting.Api.Filters;
using LuxorLMS.Reporting.Application.DTOs;
using LuxorLMS.Reporting.Application.Interfaces;
using LuxorLMS.Reporting.Application.Permissions;
using LuxorLMS.Reporting.Domain.Enums;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Reporting.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpPost]
    [RequirePermissionFilterFactory(ReportingPermissions.Create)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _reportingService.CreateReportAsync(request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(ReportingPermissions.View)]
    public async Task<IActionResult> GetReport(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reportingService.GetReportAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("user/{userId:guid}")]
    [RequirePermissionFilterFactory(ReportingPermissions.View)]
    public async Task<IActionResult> GetUserReports(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _reportingService.GetUserReportsAsync(userId, pageNumber, pageSize, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

[ApiController]
[Route("api/v1/report-templates")]
public class ReportTemplatesController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportTemplatesController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(ReportingPermissions.View)]
    public async Task<IActionResult> GetTemplates(CancellationToken cancellationToken)
    {
        var result = await _reportingService.GetTemplatesAsync(cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(ReportingPermissions.ManageTemplates)]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _reportingService.CreateTemplateAsync(request.Code, request.Name, request.ReportType, request.Format, request.TemplatePath, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

public record CreateTemplateRequest(
    string Code,
    string Name,
    ReportType ReportType,
    ExportFormat Format,
    string TemplatePath
);
