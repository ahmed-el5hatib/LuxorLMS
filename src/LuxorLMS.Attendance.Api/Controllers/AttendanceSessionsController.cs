using LuxorLMS.Attendance.Api.Authorization;
using LuxorLMS.Attendance.Api.Filters;
using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Attendance.Api.Controllers;

[ApiController]
[Route("api/v1/attendance/sessions")]
public class AttendanceSessionsController : ControllerBase
{
    private readonly IAttendanceSessionService _service;

    public AttendanceSessionsController(IAttendanceSessionService service)
    {
        _service = service;
    }

    [HttpGet("offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetByCourseOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByCourseOfferingAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("section/{sectionId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetBySection(Guid sectionId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySectionAsync(sectionId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionCreate)]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceSessionRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionCreate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionCreate)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
