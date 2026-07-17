using LuxorLMS.Kernel;
using LuxorLMS.Registration.Api.Authorization;
using LuxorLMS.Registration.Api.Filters;
using LuxorLMS.Registration.Application.DTOs;
using LuxorLMS.Registration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/registration/periods")]
public class RegistrationPeriodsController : ControllerBase
{
    private readonly IRegistrationPeriodService _service;

    public RegistrationPeriodsController(IRegistrationPeriodService service)
    {
        _service = service;
    }

    [HttpGet("semester/{semesterId:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> GetBySemester(Guid semesterId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySemesterIdAsync(semesterId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("active")]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var result = await _service.GetActiveAsync(cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationPeriodRequest request, CancellationToken cancellationToken)
    {
        var userId = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, userId ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRegistrationPeriodRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(RegistrationPermissions.PeriodManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
