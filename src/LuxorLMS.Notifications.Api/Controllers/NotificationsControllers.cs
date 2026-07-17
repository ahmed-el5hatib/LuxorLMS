using LuxorLMS.Kernel;
using LuxorLMS.Notifications.Api.Authorization;
using LuxorLMS.Notifications.Api.Filters;
using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Application.Permissions;
using LuxorLMS.Notifications.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Notifications.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    [RequirePermissionFilterFactory(NotificationsPermissions.Send)]
    public async Task<IActionResult> SendNow([FromBody] SendNotificationRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _notificationService.SendNowAsync(request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("schedule")]
    [RequirePermissionFilterFactory(NotificationsPermissions.Send)]
    public async Task<IActionResult> Schedule([FromBody] ScheduleNotificationRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = PermissionHelper.GetCurrentUserId(User) ?? Guid.Empty;
        var result = await _notificationService.ScheduleAsync(request, currentUserId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("user/{userId:guid}")]
    [RequirePermissionFilterFactory(NotificationsPermissions.View)]
    public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] int limit = 50, CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.GetUserNotificationsAsync(userId, limit, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}

[ApiController]
[Route("api/v1/notification-preferences")]
public class NotificationPreferencesController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationPreferencesController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("user/{userId:guid}")]
    [RequirePermissionFilterFactory(NotificationsPermissions.ManagePreferences)]
    public async Task<IActionResult> GetPreferences(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _notificationService.GetUserPreferencesAsync(userId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPut("user/{userId:guid}")]
    [RequirePermissionFilterFactory(NotificationsPermissions.ManagePreferences)]
    public async Task<IActionResult> UpdatePreference(Guid userId, [FromQuery] NotificationChannel channel, [FromQuery] bool enabled, CancellationToken cancellationToken)
    {
        var result = await _notificationService.UpdatePreferenceAsync(userId, channel, enabled, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Code,
        Detail = error.Description
    };
}
