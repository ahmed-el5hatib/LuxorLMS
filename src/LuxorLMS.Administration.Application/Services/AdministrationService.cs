using LuxorLMS.Administration.Application.DTOs;
using LuxorLMS.Administration.Application.Interfaces;
using LuxorLMS.Administration.Domain.Entities;
using LuxorLMS.Administration.Domain.Enums;
using LuxorLMS.Administration.Domain.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.Extensions.Logging;

namespace LuxorLMS.Administration.Application.Services;

public class AdministrationService : IAdministrationService
{
    private readonly ISystemSettingRepository _settingRepository;
    private readonly ISystemLogRepository _logRepository;
    private readonly IBackgroundJobInfoRepository _jobInfoRepository;
    private readonly IAdministrationUnitOfWork _unitOfWork;
    private readonly ILogger<AdministrationService> _logger;

    public AdministrationService(
        ISystemSettingRepository settingRepository,
        ISystemLogRepository logRepository,
        IBackgroundJobInfoRepository jobInfoRepository,
        IAdministrationUnitOfWork unitOfWork,
        ILogger<AdministrationService> logger)
    {
        _settingRepository = settingRepository;
        _logRepository = logRepository;
        _jobInfoRepository = jobInfoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PagedResult<SystemSettingDto>>> GetSettingsAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var allSettings = await _settingRepository.GetAllAsync(cancellationToken);
        var totalCount = allSettings.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var items = allSettings
            .OrderBy(s => s.Key)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapSetting)
            .ToList();

        return Result<PagedResult<SystemSettingDto>>.Success(new PagedResult<SystemSettingDto>(items, pageNumber, pageSize, totalCount, totalPages));
    }

    public async Task<Result<SystemSettingDto>> GetSettingByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _settingRepository.GetByKeyAsync(key, cancellationToken);
        if (setting is null)
            return Result<SystemSettingDto>.Failure(new Error("Administration.NotFound", "Setting not found."));

        return Result<SystemSettingDto>.Success(MapSetting(setting));
    }

    public async Task<Result<SystemSettingDto>> UpdateSettingAsync(string key, UpdateSettingRequest request, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        var setting = await _settingRepository.GetByKeyAsync(key, cancellationToken);
        if (setting is null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Value = request.Value,
                UpdatedBy = updatedBy,
                UpdatedAt = DateTime.UtcNow
            };
            await _settingRepository.AddAsync(setting, cancellationToken);
        }
        else
        {
            var oldValue = setting.Value;
            setting.Value = request.Value;
            setting.UpdatedBy = updatedBy;
            setting.UpdatedAt = DateTime.UtcNow;
            _settingRepository.Update(setting);

            _logger.LogInformation("Setting {Key} updated from {OldValue} to {NewValue} by {UserId}", key, oldValue, request.Value, updatedBy);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SystemSettingDto>.Success(MapSetting(setting));
    }

    public async Task<Result<PagedResult<SystemLogDto>>> GetLogsAsync(LogsFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var logs = await _logRepository.GetAsync(filter.Level, filter.Category, filter.From, filter.To, filter.PageNumber, filter.PageSize, cancellationToken);
        var totalCount = logs.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);
        var items = logs.Select(MapLog).ToList();

        return Result<PagedResult<SystemLogDto>>.Success(new PagedResult<SystemLogDto>(items, filter.PageNumber, filter.PageSize, totalCount, totalPages));
    }

    public async Task<Result> SeedSettingsAsync(CancellationToken cancellationToken = default)
    {
        var defaultSettings = new[]
        {
            new { Key = "app.name", Value = "LuxorLMS", Type = SettingType.String, Description = "Application display name" },
            new { Key = "app.max-upload-size-mb", Value = "50", Type = SettingType.Integer, Description = "Maximum file upload size in MB" },
            new { Key = "auth.password-min-length", Value = "10", Type = SettingType.Integer, Description = "Minimum password length" },
            new { Key = "auth.lockout-minutes", Value = "15", Type = SettingType.Integer, Description = "Account lockout duration in minutes" },
            new { Key = "notifications.fallback-to-email", Value = "true", Type = SettingType.Boolean, Description = "Fallback to email if push fails" },
            new { Key = "registration.min-credits", Value = "12", Type = SettingType.Integer, Description = "Minimum credit hours per semester" },
            new { Key = "registration.max-credits", Value = "18", Type = SettingType.Integer, Description = "Maximum credit hours per semester" },
            new { Key = "attendance.absence-warning-threshold", Value = "0.25", Type = SettingType.Integer, Description = "Absence rate threshold for warning (25%)" }
        };

        foreach (var setting in defaultSettings)
        {
            var existing = await _settingRepository.GetByKeyAsync(setting.Key, cancellationToken);
            if (existing is null)
            {
                await _settingRepository.AddAsync(new SystemSetting
                {
                    Key = setting.Key,
                    Value = setting.Value,
                    Type = setting.Type,
                    Description = setting.Description,
                    UpdatedAt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<BackgroundJobInfoDto>>> GetBackgroundJobsAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await _jobInfoRepository.GetAllAsync(cancellationToken);
        var result = jobs.Select(MapJob).ToList();
        return Result<IReadOnlyList<BackgroundJobInfoDto>>.Success(result);
    }

    private static SystemSettingDto MapSetting(SystemSetting s) => new(
        s.Id,
        s.Key,
        s.IsSensitive ? "***" : s.Value,
        s.Type.ToString(),
        s.Description,
        s.IsSensitive,
        s.UpdatedBy,
        s.UpdatedAt
    );

    private static SystemLogDto MapLog(SystemLog l) => new(
        l.Id,
        l.Level,
        l.Category,
        l.Message,
        l.Exception,
        l.Source,
        l.UserId,
        l.IpAddress,
        l.Timestamp
    );

    private static BackgroundJobInfoDto MapJob(BackgroundJobInfo j) => new(
        j.Id,
        j.JobId,
        j.Name,
        j.State,
        j.NextExecution,
        j.LastExecution,
        j.CronExpression
    );
}
