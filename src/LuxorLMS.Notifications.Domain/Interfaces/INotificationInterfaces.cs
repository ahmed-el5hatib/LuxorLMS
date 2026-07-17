using LuxorLMS.Notifications.Domain.Entities;
using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Notifications.Domain.Interfaces;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByCodeAsync(string code, NotificationChannel channel, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    void Update(NotificationTemplate template);
}

public interface INotificationMessageRepository
{
    Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationMessage>> GetByRecipientIdAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationMessage>> GetPendingScheduledAsync(DateTime before, CancellationToken cancellationToken = default);
    Task AddAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    void Update(NotificationMessage message);
}

public interface INotificationPreferenceRepository
{
    Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannel channel, CancellationToken cancellationToken = default);
    Task AddAsync(NotificationPreference preference, CancellationToken cancellationToken = default);
    void Update(NotificationPreference preference);
}

public interface INotificationsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
