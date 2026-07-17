using LuxorLMS.Academic.Api.Authorization;
using LuxorLMS.Academic.Api.Filters;
using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Academic.Api.Controllers;

[ApiController]
[Route("api/v1/academic/offerings/{courseOfferingId:guid}/sections")]
public class SectionsController : ControllerBase
{
    private readonly ISectionService _service;

    public SectionsController(ISectionService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> GetByOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> GetById(Guid courseOfferingId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> Create(Guid courseOfferingId, [FromBody] CreateSectionRequest request, CancellationToken cancellationToken)
    {
        if (request.CourseOfferingId != courseOfferingId)
            return BadRequest(ToProblem(new Error("Section.RouteMismatch", "Route courseOfferingId does not match body.")));

        var userId = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, userId ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { courseOfferingId, id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> Update(Guid courseOfferingId, Guid id, [FromBody] UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> Delete(Guid courseOfferingId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    [HttpPost("{id:guid}/members")]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> AddMember(Guid courseOfferingId, Guid id, [FromBody] AddSectionMemberRequest request, CancellationToken cancellationToken)
    {
        var userId = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.AddMemberAsync(id, request, userId ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/members/{studentId:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.SectionManage)]
    public async Task<IActionResult> RemoveMember(Guid courseOfferingId, Guid id, Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.RemoveMemberAsync(id, studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
