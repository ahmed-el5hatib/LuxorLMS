using LuxorLMS.Notifications.Domain.Entities;
using LuxorLMS.Notifications.Domain.Enums;
using LuxorLMS.Notifications.Domain.Interfaces;
using LuxorLMS.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Notifications.Infrastructure.Repositories;

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly LuxorLMSNotificationsDbContext _context;

    public NotificationTemplateRepository(LuxorLMSNotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationTemplate?> GetByCodeAsync(string code, NotificationChannel channel, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Code == code && t.Channel == channel, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.NotificationTemplates.AddAsync(template, cancellationToken);
    }

    public void Update(NotificationTemplate template)
    {
        _context.NotificationTemplates.Update(template);
    }
}

public class NotificationMessageRepository : INotificationMessageRepository
{
    private readonly LuxorLMSNotificationsDbContext _context;

    public NotificationMessageRepository(LuxorLMSNotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationMessages
            .Include(m => m.Template)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetByRecipientIdAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationMessages
            .Where(m => m.RecipientUserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetPendingScheduledAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationMessages
            .Where(m => m.Status == NotificationStatus.Queued && m.ScheduledAt != null && m.ScheduledAt <= before)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        await _context.NotificationMessages.AddAsync(message, cancellationToken);
    }

    public void Update(NotificationMessage message)
    {
        _context.NotificationMessages.Update(message);
    }
}

public class NotificationPreferenceRepository : INotificationPreferenceRepository
{
    private readonly LuxorLMSNotificationsDbContext _context;

    public NotificationPreferenceRepository(LuxorLMSNotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationPreferences.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannel channel, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId && p.Channel == channel, cancellationToken);
    }

    public async Task AddAsync(NotificationPreference preference, CancellationToken cancellationToken = default)
    {
        await _context.NotificationPreferences.AddAsync(preference, cancellationToken);
    }

    public void Update(NotificationPreference preference)
    {
        _context.NotificationPreferences.Update(preference);
    }
}

public class NotificationsUnitOfWork : INotificationsUnitOfWork
{
    private readonly LuxorLMSNotificationsDbContext _context;

    public NotificationsUnitOfWork(LuxorLMSNotificationsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
