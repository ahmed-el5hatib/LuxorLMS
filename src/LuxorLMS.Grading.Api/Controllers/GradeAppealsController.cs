using LuxorLMS.Grading.Api.Authorization;
using LuxorLMS.Grading.Api.Filters;
using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Grading.Api.Controllers;

[ApiController]
[Route("api/v1/grading/appeals")]
public class GradeAppealsController : ControllerBase
{
    private readonly IGradeAppealService _service;

    public GradeAppealsController(IGradeAppealService service)
    {
        _service = service;
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.AppealSubmit)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        // Students may only view their own appeals.
        var current = PermissionHelper.GetCurrentUserId(User);
        if (current != studentId) return Forbid();

        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("grade/{studentGradeId:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.AppealResolve)]
    public async Task<IActionResult> GetByGrade(Guid studentGradeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByGradeAsync(studentGradeId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.AppealResolve)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(GradingPermissions.AppealSubmit)]
    public async Task<IActionResult> Create([FromBody] CreateGradeAppealRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/resolve")]
    [RequirePermissionFilterFactory(GradingPermissions.AppealResolve)]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveGradeAppealRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.ResolveAsync(id, request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
