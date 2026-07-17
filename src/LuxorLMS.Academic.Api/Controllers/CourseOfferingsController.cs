using LuxorLMS.Academic.Api.Authorization;
using LuxorLMS.Academic.Api.Filters;
using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Academic.Api.Controllers;

[ApiController]
[Route("api/v1/academic/offerings")]
public class CourseOfferingsController : ControllerBase
{
    private readonly ICourseOfferingService _service;

    public CourseOfferingsController(ICourseOfferingService service)
    {
        _service = service;
    }

    [HttpGet("semester/{semesterId:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> GetBySemester(Guid semesterId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySemesterIdAsync(semesterId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("course/{courseId:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> GetByCourse(Guid courseId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByCourseIdAsync(courseId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> Create([FromBody] CreateCourseOfferingRequest request, CancellationToken cancellationToken)
    {
        var userId = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, userId ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseOfferingRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.OfferingManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
