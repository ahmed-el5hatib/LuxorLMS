namespace LuxorLMS.Administration.Application.DTOs;

public record SystemSettingDto(
    Guid Id,
    string Key,
    string Value,
    string Type,
    string? Description,
    bool IsSensitive,
    Guid? UpdatedBy,
    DateTime UpdatedAt
);

public record UpdateSettingRequest(
    string Value
);

public record SystemLogDto(
    Guid Id,
    string Level,
    string Category,
    string Message,
    string? Exception,
    string? Source,
    Guid? UserId,
    string? IpAddress,
    DateTime Timestamp
);

public record BackgroundJobInfoDto(
    Guid Id,
    string JobId,
    string Name,
    string State,
    DateTime? NextExecution,
    DateTime? LastExecution,
    string? CronExpression
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);

public record LogsFilterRequest(
    string? Level,
    string? Category,
    DateTime? From,
    DateTime? To,
    int PageNumber = 1,
    int PageSize = 50
);
