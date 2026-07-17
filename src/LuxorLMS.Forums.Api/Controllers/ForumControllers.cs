using LuxorLMS.Forums.Api.Authorization;
using LuxorLMS.Forums.Api.Filters;
using LuxorLMS.Forums.Application.DTOs;
using LuxorLMS.Forums.Application.Interfaces;
using LuxorLMS.Forums.Application.Permissions;
using LuxorLMS.Forums.Domain.Enums;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Forums.Api.Controllers;

[ApiController]
[Route("api/v1/forum-topics")]
public class ForumTopicsController : ControllerBase
{
    private readonly IForumService _forumService;

    public ForumTopicsController(IForumService forumService)
    {
        _forumService = forumService;
    }

    [HttpPost]
    [RequirePermissionFilterFactory(ForumsPermissions.Write)]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.CreateTopicAsync(request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(ForumsPermissions.Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.GetTopicByIdAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("course-offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(ForumsPermissions.Read)]
    public async Task<IActionResult> GetByCourseOffering(Guid courseOfferingId, [FromQuery] DateTime? cursor, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.GetTopicsByCourseOfferingAsync(courseOfferingId, cursor, pageSize, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("course-offering/{courseOfferingId:guid}/search")]
    [RequirePermissionFilterFactory(ForumsPermissions.Read)]
    public async Task<IActionResult> SearchTopics(Guid courseOfferingId, [FromQuery] string q, [FromQuery] DateTime? cursor, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ToProblem(new Error("Forums.InvalidSearch", "Search term is required.")));

        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.SearchTopicsAsync(courseOfferingId, q, cursor, pageSize, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPut("{id:guid}/pin")]
    [RequirePermissionFilterFactory(ForumsPermissions.PinLock)]
    public async Task<IActionResult> TogglePin(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.TogglePinAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    [HttpPut("{id:guid}/lock")]
    [RequirePermissionFilterFactory(ForumsPermissions.PinLock)]
    public async Task<IActionResult> ToggleLock(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.ToggleLockAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(ForumsPermissions.Moderate)]
    public async Task<IActionResult> DeleteTopic(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.DeleteTopicAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

[ApiController]
[Route("api/v1/forum-posts")]
public class ForumPostsController : ControllerBase
{
    private readonly IForumService _forumService;

    public ForumPostsController(IForumService forumService)
    {
        _forumService = forumService;
    }

    [HttpPost]
    [RequirePermissionFilterFactory(ForumsPermissions.Write)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.CreatePostAsync(request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("topic/{topicId:guid}")]
    [RequirePermissionFilterFactory(ForumsPermissions.Read)]
    public async Task<IActionResult> GetByTopic(Guid topicId, [FromQuery] DateTime? cursor, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.GetPostsByTopicAsync(topicId, cursor, pageSize, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("topic/{topicId:guid}/search")]
    [RequirePermissionFilterFactory(ForumsPermissions.Read)]
    public async Task<IActionResult> SearchPosts(Guid topicId, [FromQuery] string q, [FromQuery] DateTime? cursor, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ToProblem(new Error("Forums.InvalidSearch", "Search term is required.")));

        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.SearchPostsAsync(topicId, q, cursor, pageSize, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPut("{id:guid}/moderate")]
    [RequirePermissionFilterFactory(ForumsPermissions.Moderate)]
    public async Task<IActionResult> ModeratePost(Guid id, [FromQuery] ForumModerationStatus status, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.ModeratePostAsync(id, status, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(ForumsPermissions.Moderate)]
    public async Task<IActionResult> DeletePost(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _forumService.DeletePostAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}
