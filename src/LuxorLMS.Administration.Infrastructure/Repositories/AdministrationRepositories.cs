using LuxorLMS.Administration.Domain.Entities;
using LuxorLMS.Administration.Domain.Interfaces;
using LuxorLMS.Administration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Administration.Infrastructure.Repositories;

public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly LuxorLMSAdministrationDbContext _context;

    public SystemSettingRepository(LuxorLMSAdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        await _context.SystemSettings.AddAsync(setting, cancellationToken);
    }

    public void Update(SystemSetting setting)
    {
        _context.SystemSettings.Update(setting);
    }
}

public class SystemLogRepository : ISystemLogRepository
{
    private readonly LuxorLMSAdministrationDbContext _context;

    public SystemLogRepository(LuxorLMSAdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SystemLog>> GetAsync(string? level, string? category, DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.SystemLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(l => l.Level == level);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(l => l.Category == category);

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        return await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SystemLog log, CancellationToken cancellationToken = default)
    {
        await _context.SystemLogs.AddAsync(log, cancellationToken);
    }
}

public class BackgroundJobInfoRepository : IBackgroundJobInfoRepository
{
    private readonly LuxorLMSAdministrationDbContext _context;

    public BackgroundJobInfoRepository(LuxorLMSAdministrationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BackgroundJobInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BackgroundJobInfos.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BackgroundJobInfo jobInfo, CancellationToken cancellationToken = default)
    {
        await _context.BackgroundJobInfos.AddAsync(jobInfo, cancellationToken);
    }

    public void Update(BackgroundJobInfo jobInfo)
    {
        _context.BackgroundJobInfos.Update(jobInfo);
    }
}

public class AdministrationUnitOfWork : IAdministrationUnitOfWork
{
    private readonly LuxorLMSAdministrationDbContext _context;

    public AdministrationUnitOfWork(LuxorLMSAdministrationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
