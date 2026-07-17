using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Notifications.Domain.Entities;

public class NotificationTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string Culture { get; set; } = "en-US";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class NotificationMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TemplateId { get; set; }
    public Guid RecipientUserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Error { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NotificationTemplate? Template { get; set; }
}

public class NotificationPreference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool Enabled { get; set; } = true;
}
