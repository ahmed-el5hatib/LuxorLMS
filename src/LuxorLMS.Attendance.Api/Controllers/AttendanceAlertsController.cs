using LuxorLMS.Attendance.Api.Authorization;
using LuxorLMS.Attendance.Api.Filters;
using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Attendance.Api.Controllers;

[ApiController]
[Route("api/v1/attendance/alerts")]
public class AttendanceAlertsController : ControllerBase
{
    private readonly IAttendanceAlertService _service;

    public AttendanceAlertsController(IAttendanceAlertService service)
    {
        _service = service;
    }

    [HttpGet("offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.AlertView)]
    public async Task<IActionResult> GetByCourseOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByCourseOfferingAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(AttendancePermissions.AlertView)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
