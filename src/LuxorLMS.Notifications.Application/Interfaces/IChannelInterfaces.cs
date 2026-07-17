using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Domain.Entities;
using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Notifications.Application.Interfaces;

public interface IChannelSender
{
    NotificationChannel Channel { get; }
    Task<bool> SendAsync(NotificationMessage message, UserContactInfo contactInfo, CancellationToken cancellationToken = default);
}

public interface IUserNotificationGateway
{
    Task<UserContactInfo?> GetContactInfoAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserContactInfo>> GetCourseOfferingRecipientsAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
}
