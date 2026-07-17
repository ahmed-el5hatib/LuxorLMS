using LuxorLMS.Grading.Api.Authorization;
using LuxorLMS.Grading.Api.Filters;
using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Grading.Api.Controllers;

[ApiController]
[Route("api/v1/grading/schema")]
public class GradeSchemaController : ControllerBase
{
    private readonly IGradeSchemaService _service;

    public GradeSchemaController(IGradeSchemaService service)
    {
        _service = service;
    }

    // ----- Categories -----

    [HttpGet("offerings/{courseOfferingId:guid}/categories")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetCategories(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetCategoriesByOfferingAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("categories/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetCategoryByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("categories")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateGradeCategoryRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateCategoryAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetCategoryById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("categories/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateGradeCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateCategoryAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("categories/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteCategoryAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    // ----- Components -----

    [HttpGet("categories/{gradeCategoryId:guid}/components")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetComponents(Guid gradeCategoryId, CancellationToken cancellationToken)
    {
        var result = await _service.GetComponentsByCategoryAsync(gradeCategoryId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("components/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.GradeView)]
    public async Task<IActionResult> GetComponentById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetComponentByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("components")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> CreateComponent([FromBody] CreateGradeComponentRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateComponentAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetComponentById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("components/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> UpdateComponent(Guid id, [FromBody] UpdateGradeComponentRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateComponentAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("components/{id:guid}")]
    [RequirePermissionFilterFactory(GradingPermissions.SchemaManage)]
    public async Task<IActionResult> DeleteComponent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteComponentAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
