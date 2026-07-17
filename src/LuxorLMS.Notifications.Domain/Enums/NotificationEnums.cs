namespace LuxorLMS.Notifications.Domain.Enums;

public enum NotificationChannel
{
    InApp = 1,
    Email = 2,
    Sms = 3,
    Push = 4
}

public enum NotificationStatus
{
    Queued = 1,
    Sent = 2,
    Failed = 3,
    Cancelled = 4
}
