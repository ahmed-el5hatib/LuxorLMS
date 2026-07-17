using LuxorLMS.Analytics.Application.DTOs;
using LuxorLMS.Analytics.Application.Interfaces;
using LuxorLMS.Analytics.Domain.Entities;
using LuxorLMS.Analytics.Domain.Enums;
using LuxorLMS.Analytics.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Analytics.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsKpiRepository _kpiRepository;
    private readonly IGpaTrendRepository _gpaTrendRepository;
    private readonly IGradeDistributionRepository _gradeDistributionRepository;
    private readonly IServerHealthRepository _serverHealthRepository;
    private readonly IAnalyticsUnitOfWork _unitOfWork;

    public AnalyticsService(
        IAnalyticsKpiRepository kpiRepository,
        IGpaTrendRepository gpaTrendRepository,
        IGradeDistributionRepository gradeDistributionRepository,
        IServerHealthRepository serverHealthRepository,
        IAnalyticsUnitOfWork unitOfWork)
    {
        _kpiRepository = kpiRepository;
        _gpaTrendRepository = gpaTrendRepository;
        _gradeDistributionRepository = gradeDistributionRepository;
        _serverHealthRepository = serverHealthRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<AnalyticsKpiDto>>> GetKpisAsync(AnalyticsFilterRequest filter, CancellationToken cancellationToken = default)
    {
        MetricType? metricType = null;
        if (!string.IsNullOrWhiteSpace(filter.MetricType) && Enum.TryParse<MetricType>(filter.MetricType, true, out var parsedMetric))
            metricType = parsedMetric;

        TimeRange? timeRange = null;
        if (!string.IsNullOrWhiteSpace(filter.TimeRange) && Enum.TryParse<TimeRange>(filter.TimeRange, true, out var parsedRange))
            timeRange = parsedRange;

        var items = await _kpiRepository.GetByFilterAsync(
            filter.CourseOfferingId,
            filter.DepartmentId,
            filter.ProgramId,
            metricType,
            timeRange,
            filter.From,
            filter.To,
            filter.PageNumber,
            filter.PageSize,
            cancellationToken);

        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        var result = new PagedResult<AnalyticsKpiDto>(
            items.Select(MapKpi).ToList(),
            filter.PageNumber,
            filter.PageSize,
            totalCount,
            totalPages);

        return Result<PagedResult<AnalyticsKpiDto>>.Success(result);
    }

    public async Task<Result<AnalyticsKpiDto>> GetLatestKpiAsync(string key, Guid? courseOfferingId, CancellationToken cancellationToken = default)
    {
        var all = await _kpiRepository.GetByFilterAsync(courseOfferingId, null, null, null, null, null, null, 1, 1, cancellationToken);
        var latest = all.FirstOrDefault(k => k.Key == key);
        if (latest is null)
            return Result<AnalyticsKpiDto>.Failure(new Error("Analytics.NotFound", "KPI not found."));

        return Result<AnalyticsKpiDto>.Success(MapKpi(latest));
    }

    public async Task<Result<IReadOnlyList<GpaTrendDto>>> GetStudentGpaTrendAsync(Guid studentId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var trends = await _gpaTrendRepository.GetByStudentAsync(studentId, pageNumber, pageSize, cancellationToken);
        var result = trends.Select(MapGpaTrend).ToList();
        return Result<IReadOnlyList<GpaTrendDto>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<GradeDistributionDto>>> GetCourseGradeDistributionAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var distributions = await _gradeDistributionRepository.GetByCourseOfferingAsync(courseOfferingId, cancellationToken);
        var result = distributions.Select(MapGradeDistribution).ToList();
        return Result<IReadOnlyList<GradeDistributionDto>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<ServerHealthMetricDto>>> GetServerHealthAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        var metrics = await _serverHealthRepository.GetRecentAsync(limit, cancellationToken);
        var result = metrics.Select(MapServerHealth).ToList();
        return Result<IReadOnlyList<ServerHealthMetricDto>>.Success(result);
    }

    public async Task<Result<AnalyticsKpiDto>> RecordServerHealthAsync(string metricName, decimal value, string? unit, string status, string? details, CancellationToken cancellationToken = default)
    {
        var metric = new ServerHealthMetric
        {
            MetricName = metricName,
            Value = value,
            Unit = unit,
            Status = status,
            Details = details,
            RecordedAt = DateTime.UtcNow
        };

        await _serverHealthRepository.AddAsync(metric, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var kpi = new AnalyticsKpi
        {
            Key = $"server.{metricName.ToLowerInvariant()}",
            Name = metricName,
            Value = value,
            Unit = unit,
            MetricType = MetricType.ServerHealth,
            TimeRange = TimeRange.Daily,
            CalculatedAt = DateTime.UtcNow,
            PeriodStart = DateTime.UtcNow.Date,
            PeriodEnd = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1),
            Metadata = details
        };

        await _kpiRepository.AddAsync(kpi, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AnalyticsKpiDto>.Success(MapKpi(kpi));
    }

    private static AnalyticsKpiDto MapKpi(AnalyticsKpi k) => new(
        k.Id,
        k.Key,
        k.Name,
        k.Value,
        k.Unit,
        k.MetricType.ToString(),
        k.TimeRange.ToString(),
        k.CourseOfferingId,
        k.DepartmentId,
        k.ProgramId,
        k.CalculatedAt,
        k.PeriodStart,
        k.PeriodEnd,
        k.Metadata
    );

    private static GpaTrendDto MapGpaTrend(GpaTrend t) => new(
        t.Id,
        t.StudentId,
        t.CourseOfferingId,
        t.SemesterGpa,
        t.CumulativeGpa,
        t.SemesterNumber,
        t.CalculatedAt
    );

    private static GradeDistributionDto MapGradeDistribution(GradeDistribution d) => new(
        d.Id,
        d.CourseOfferingId,
        d.GradeLetter,
        d.StudentCount,
        d.Percentage,
        d.MinScore,
        d.MaxScore,
        d.AverageScore,
        d.CalculatedAt
    );

    private static ServerHealthMetricDto MapServerHealth(ServerHealthMetric m) => new(
        m.Id,
        m.MetricName,
        m.Value,
        m.Unit,
        m.Status,
        m.RecordedAt,
        m.Details
    );
}
