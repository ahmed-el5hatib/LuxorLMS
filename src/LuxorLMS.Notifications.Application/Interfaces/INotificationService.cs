using LuxorLMS.Kernel;
using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Notifications.Application.Interfaces;

public interface INotificationService
{
    Task<Result<NotificationMessageDto>> SendNowAsync(SendNotificationRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<NotificationMessageDto>> ScheduleAsync(ScheduleNotificationRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<int>> SendToCourseOfferingAsync(string templateCode, Guid courseOfferingId, Dictionary<string, string> placeholders, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<NotificationMessageDto>>> GetUserNotificationsAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<NotificationPreferenceDto>>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdatePreferenceAsync(Guid userId, NotificationChannel channel, bool enabled, CancellationToken cancellationToken = default);
    Task DispatchMessageAsync(Guid messageId, CancellationToken cancellationToken = default);
}
