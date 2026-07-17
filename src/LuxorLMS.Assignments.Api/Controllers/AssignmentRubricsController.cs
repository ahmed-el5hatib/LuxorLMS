using LuxorLMS.Assignments.Api.Authorization;
using LuxorLMS.Assignments.Api.Filters;
using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Assignments.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Assignments.Api.Controllers;

[ApiController]
[Route("api/v1/assignments/{assignmentId:guid}/rubric")]
public class AssignmentRubricsController : ControllerBase
{
    private readonly IAssignmentService _service;

    public AssignmentRubricsController(IAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AssignmentPermissions.View)]
    public async Task<IActionResult> GetByAssignment(Guid assignmentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRubricAsync(assignmentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AssignmentPermissions.Create)]
    public async Task<IActionResult> Add(Guid assignmentId, [FromBody] CreateAssignmentRubricRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.AddRubricAsync(request with { AssignmentId = assignmentId }, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetByAssignment), new { assignmentId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Edit)]
    public async Task<IActionResult> Update(Guid assignmentId, Guid id, [FromBody] UpdateAssignmentRubricRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateRubricAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AssignmentPermissions.Edit)]
    public async Task<IActionResult> Delete(Guid assignmentId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteRubricAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
