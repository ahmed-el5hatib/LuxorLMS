using LuxorLMS.Attendance.Api.Authorization;
using LuxorLMS.Attendance.Api.Filters;
using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Attendance.Api.Controllers;

[ApiController]
[Route("api/v1/attendance/records")]
public class AttendanceRecordsController : ControllerBase
{
    private readonly IAttendanceRecordService _service;

    public AttendanceRecordsController(IAttendanceRecordService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("session/{attendanceSessionId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetBySession(Guid attendanceSessionId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySessionAsync(attendanceSessionId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.SessionView)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("mark")]
    [RequirePermissionFilterFactory(AttendancePermissions.Mark)]
    public async Task<IActionResult> Mark([FromBody] MarkAttendanceRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.MarkAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
