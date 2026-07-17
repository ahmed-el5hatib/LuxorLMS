using LuxorLMS.Analytics.Api.Filters;
using LuxorLMS.Analytics.Application.DTOs;
using LuxorLMS.Analytics.Application.Interfaces;
using LuxorLMS.Analytics.Application.Permissions;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Analytics.Api.Controllers;

[ApiController]
[Route("api/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("kpis")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.View)]
    public async Task<IActionResult> GetKpis([FromQuery] AnalyticsFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetKpisAsync(filter, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("kpis/latest")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.View)]
    public async Task<IActionResult> GetLatestKpi([FromQuery] string key, [FromQuery] Guid? courseOfferingId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            return BadRequest(ToProblem(new Error("Analytics.InvalidKey", "Key is required.")));

        var result = await _analyticsService.GetLatestKpiAsync(key, courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("gpa-trends/student/{studentId:guid}")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.View)]
    public async Task<IActionResult> GetStudentGpaTrend(Guid studentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _analyticsService.GetStudentGpaTrendAsync(studentId, pageNumber, pageSize, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("grade-distributions/course-offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.View)]
    public async Task<IActionResult> GetCourseGradeDistribution(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetCourseGradeDistributionAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("server-health")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.ViewServerHealth)]
    public async Task<IActionResult> GetServerHealth([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        var result = await _analyticsService.GetServerHealthAsync(limit, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("server-health")]
    [RequirePermissionFilterFactory(AnalyticsPermissions.Manage)]
    public async Task<IActionResult> RecordServerHealth([FromBody] RecordServerHealthRequest request, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.RecordServerHealthAsync(request.MetricName, request.Value, request.Unit, request.Status, request.Details, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

public record RecordServerHealthRequest(
    string MetricName,
    decimal Value,
    string? Unit,
    string Status,
    string? Details
);
