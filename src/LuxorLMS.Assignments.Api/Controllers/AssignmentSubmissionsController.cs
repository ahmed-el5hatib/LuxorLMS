using LuxorLMS.Assignments.Api.Authorization;
using LuxorLMS.Assignments.Api.Filters;
using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Assignments.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Assignments.Api.Controllers;

[ApiController]
[Route("api/v1/assignments/submissions")]
public class AssignmentSubmissionsController : ControllerBase
{
    private readonly IAssignmentSubmissionService _service;

    public AssignmentSubmissionsController(IAssignmentSubmissionService service)
    {
        _service = service;
    }

    [HttpGet("assignment/{assignmentId:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.View)]
    public async Task<IActionResult> GetByAssignment(Guid assignmentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByAssignmentAsync(assignmentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.View)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.View)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AssignmentPermissions.Submit)]
    public async Task<IActionResult> Submit([FromBody] SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.SubmitAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/grade")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Grade)]
    public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.GradeAsync(id, request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/return")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Grade)]
    public async Task<IActionResult> Return(Guid id, [FromBody] ReturnSubmissionRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.ReturnAsync(id, request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Submit)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    [HttpGet("{submissionId:guid}/files")]
    [RequirePermissionFilterFactory(AssignmentPermissions.View)]
    public async Task<IActionResult> GetFiles(Guid submissionId, CancellationToken cancellationToken)
    {
        var result = await _service.GetFilesAsync(submissionId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{submissionId:guid}/files")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Submit)]
    public async Task<IActionResult> AddFile(Guid submissionId, [FromBody] AddAssignmentFileRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.AddFileAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetFiles), new { submissionId }, result.Value);
    }

    [HttpDelete("files/{fileId:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Submit)]
    public async Task<IActionResult> DeleteFile(Guid fileId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteFileAsync(fileId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
