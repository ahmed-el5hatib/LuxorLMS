using LuxorLMS.Administration.Application.DTOs;
using LuxorLMS.Administration.Domain.Entities;
using LuxorLMS.Administration.Domain.Enums;
using LuxorLMS.Administration.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Administration.Application.Interfaces;

public interface IAdministrationService
{
    Task<Result<PagedResult<SystemSettingDto>>> GetSettingsAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<Result<SystemSettingDto>> GetSettingByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<SystemSettingDto>> UpdateSettingAsync(string key, UpdateSettingRequest request, Guid updatedBy, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<SystemLogDto>>> GetLogsAsync(LogsFilterRequest filter, CancellationToken cancellationToken = default);
    Task<Result> SeedSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<BackgroundJobInfoDto>>> GetBackgroundJobsAsync(CancellationToken cancellationToken = default);
}
