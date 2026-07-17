using LuxorLMS.Academic.Api.Authorization;
using LuxorLMS.Academic.Api.Filters;
using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Academic.Api.Controllers;

[ApiController]
[Route("api/v1/academic/faculties")]
public class FacultiesController : ControllerBase
{
    private readonly IFacultyService _service;

    public FacultiesController(IFacultyService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermissionFilterFactory(AcademicPermissions.FacultyRead)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.FacultyRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(AcademicPermissions.FacultyManage)]
    public async Task<IActionResult> Create([FromBody] CreateFacultyRequest request, CancellationToken cancellationToken)
    {
        var userId = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, userId ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.FacultyManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacultyRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(AcademicPermissions.FacultyManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
