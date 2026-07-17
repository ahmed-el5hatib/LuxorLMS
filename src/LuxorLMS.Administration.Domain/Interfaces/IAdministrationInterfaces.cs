using LuxorLMS.Administration.Domain.Entities;

namespace LuxorLMS.Administration.Domain.Interfaces;

public interface ISystemSettingRepository
{
    Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    void Update(SystemSetting setting);
}

public interface ISystemLogRepository
{
    Task<IReadOnlyList<SystemLog>> GetAsync(string? level, string? category, DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task AddAsync(SystemLog log, CancellationToken cancellationToken = default);
}

public interface IBackgroundJobInfoRepository
{
    Task<IReadOnlyList<BackgroundJobInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BackgroundJobInfo jobInfo, CancellationToken cancellationToken = default);
    void Update(BackgroundJobInfo jobInfo);
}

public interface IAdministrationUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
