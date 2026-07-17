using LuxorLMS.Kernel;
using LuxorLMS.Registration.Api.Authorization;
using LuxorLMS.Registration.Api.Filters;
using LuxorLMS.Registration.Application.DTOs;
using LuxorLMS.Registration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/registration/enrollments")]
public class CourseEnrollmentsController : ControllerBase
{
    private readonly ICourseEnrollmentService _service;

    public CourseEnrollmentsController(ICourseEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentRegister)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        // Students may only view their own enrollments.
        var current = PermissionHelper.GetCurrentUserId(User);
        if (current != studentId) return Forbid();

        var result = await _service.GetByStudentIdAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("semester/{semesterId:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentApprove)]
    public async Task<IActionResult> GetBySemester(Guid semesterId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySemesterIdAsync(semesterId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentRegister)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("register")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentRegister)]
    public async Task<IActionResult> Register([FromBody] RegisterCourseRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        if (current != request.StudentId) return Forbid();

        var result = await _service.RegisterAsync(request, current ?? Guid.Empty, cancellationToken: cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/approve")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentApprove)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveCourseEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.ApproveAsync(id, request.Approve, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/withdraw")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentRegister)]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.WithdrawAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.EnrollmentApprove)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
