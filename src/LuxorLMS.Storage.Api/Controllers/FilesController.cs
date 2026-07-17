using LuxorLMS.Kernel;
using LuxorLMS.Storage.Api.Authorization;
using LuxorLMS.Storage.Api.Filters;
using LuxorLMS.Storage.Application.DTOs;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Application.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Storage.Api.Controllers;

[ApiController]
[Route("api/v1/storage/files")]
public class FilesController : ControllerBase
{
    private readonly IStorageService _storageService;

    public FilesController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [RequirePermissionFilterFactory(StoragePermissions.Write)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid ownerId, [FromForm] Guid? courseOfferingId, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "Invalid File", Detail = "File data is required." });

        var userId = PermissionHelper.GetCurrentUserId(User) ?? ownerId;
        using var stream = file.OpenReadStream();

        var request = new UploadFileRequest(
            ownerId,
            courseOfferingId,
            file.FileName,
            file.ContentType,
            stream,
            file.Length
        );

        var result = await _storageService.UploadAsync(request, userId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/versions")]
    [RequirePermissionFilterFactory(StoragePermissions.Write)]
    public async Task<IActionResult> UploadNewVersion(Guid id, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "Invalid File", Detail = "File data is required." });

        var userId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        using var stream = file.OpenReadStream();

        var result = await _storageService.UploadNewVersionAsync(id, stream, file.Length, userId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}/url")]
    [RequirePermissionFilterFactory(StoragePermissions.Read)]
    public async Task<IActionResult> GetSignedUrl(Guid id, [FromQuery] int? expiresInMinutes, CancellationToken cancellationToken)
    {
        var duration = expiresInMinutes.HasValue ? TimeSpan.FromMinutes(expiresInMinutes.Value) : (TimeSpan?)null;
        var result = await _storageService.GetSignedUrlAsync(id, duration, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(new { url = result.Value });
    }

    [HttpGet("{id:guid}/versions")]
    [RequirePermissionFilterFactory(StoragePermissions.Read)]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storageService.GetVersionsAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(StoragePermissions.Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storageService.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(StoragePermissions.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storageService.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}
