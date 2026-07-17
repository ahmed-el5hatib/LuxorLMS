using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Notifications.Application.DTOs;

public record NotificationTemplateDto(
    Guid Id,
    string Code,
    NotificationChannel Channel,
    string Subject,
    string BodyTemplate,
    string Culture
);

public record NotificationMessageDto(
    Guid Id,
    Guid? TemplateId,
    Guid RecipientUserId,
    NotificationChannel Channel,
    string Title,
    string Body,
    NotificationStatus Status,
    DateTime? ScheduledAt,
    DateTime? SentAt,
    string? Error,
    DateTime CreatedAt
);

public record NotificationPreferenceDto(
    Guid Id,
    Guid UserId,
    NotificationChannel Channel,
    bool Enabled
);

public record SendNotificationRequest(
    Guid RecipientUserId,
    NotificationChannel Channel,
    string Title,
    string Body,
    string? TemplateCode = null,
    Dictionary<string, string>? Placeholders = null
);

public record ScheduleNotificationRequest(
    Guid RecipientUserId,
    NotificationChannel Channel,
    string Title,
    string Body,
    DateTime ScheduledAt,
    string? TemplateCode = null,
    Dictionary<string, string>? Placeholders = null
);

public record UserContactInfo(
    Guid UserId,
    string? Email,
    string? PhoneNumber,
    string? PushToken
);
