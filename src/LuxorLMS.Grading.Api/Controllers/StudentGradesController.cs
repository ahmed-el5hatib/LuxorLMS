using LuxorLMS.Grading.Api.Authorization;
using LuxorLMS.Grading.Api.Filters;
using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Grading.Api.Controllers;

[ApiController]
[Route("api/v1/grading/grades")]
public class StudentGradesController : ControllerBase
{
    private readonly IStudentGradeService _service;

    public StudentGradesController(IStudentGradeService service)
    {
        _service = service;
    }

    [HttpGet("offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetByOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByOfferingAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(GradingPermissions.GradeEnter)]
    public async Task<IActionResult> Enter([FromBody] EnterGradeRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.EnterAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeEnter)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradeRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/submit")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeEnter)]
    public async Task<IActionResult> SubmitForApproval(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.SubmitForApprovalAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/dept-head-approve")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeApprove)]
    public async Task<IActionResult> DeptHeadApprove(Guid id, [FromBody] DeptHeadApproveRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.DeptHeadApproveAsync(id, request.Approve, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/publish")]
    [RequirePermissionFilterFactory(GradingPermissions.GradePublish)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.PublishAsync(id, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeEnter)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}

public record DeptHeadApproveRequest(bool Approve);
