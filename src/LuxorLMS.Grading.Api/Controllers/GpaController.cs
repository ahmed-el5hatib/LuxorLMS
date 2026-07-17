using LuxorLMS.Grading.Api.Authorization;
using LuxorLMS.Grading.Api.Filters;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Grading.Api.Controllers;

[ApiController]
[Route("api/v1/grading/gpa")]
public class GpaController : ControllerBase
{
    private readonly IGpaService _service;

    public GpaController(IGpaService service)
    {
        _service = service;
    }

    [HttpGet("student/{studentId:guid}/cumulative")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetCumulative(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetCumulativeGpaAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}/semester/{semesterId:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetSemester(Guid studentId, Guid semesterId, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemesterGpaAsync(studentId, semesterId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("student/{studentId:guid}/recalculate")]
    [RequirePermissionFilterFactory(GradingPermissions.GradePublish)]
    public async Task<IActionResult> Recalculate(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.RecalculateAndPersistCgpaAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
