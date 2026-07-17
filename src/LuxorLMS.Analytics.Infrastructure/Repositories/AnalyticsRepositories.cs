using LuxorLMS.Analytics.Domain.Entities;
using LuxorLMS.Analytics.Domain.Enums;
using LuxorLMS.Analytics.Domain.Interfaces;
using LuxorLMS.Analytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Analytics.Infrastructure.Repositories;

public class AnalyticsKpiRepository : IAnalyticsKpiRepository
{
    private readonly LuxorLMSAnalyticsDbContext _context;

    public AnalyticsKpiRepository(LuxorLMSAnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsKpi?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AnalyticsKpis.FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AnalyticsKpi>> GetByFilterAsync(Guid? courseOfferingId, Guid? departmentId, Guid? programId, MetricType? metricType, TimeRange? timeRange, DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.AnalyticsKpis.AsQueryable();

        if (courseOfferingId.HasValue)
            query = query.Where(k => k.CourseOfferingId == courseOfferingId);

        if (departmentId.HasValue)
            query = query.Where(k => k.DepartmentId == departmentId);

        if (programId.HasValue)
            query = query.Where(k => k.ProgramId == programId);

        if (metricType.HasValue)
            query = query.Where(k => k.MetricType == metricType);

        if (timeRange.HasValue)
            query = query.Where(k => k.TimeRange == timeRange);

        if (from.HasValue)
            query = query.Where(k => k.PeriodStart >= from.Value);

        if (to.HasValue)
            query = query.Where(k => k.PeriodEnd <= to.Value);

        return await query
            .OrderByDescending(k => k.CalculatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AnalyticsKpi kpi, CancellationToken cancellationToken = default)
    {
        await _context.AnalyticsKpis.AddAsync(kpi, cancellationToken);
    }

    public void Update(AnalyticsKpi kpi)
    {
        _context.AnalyticsKpis.Update(kpi);
    }
}

public class GpaTrendRepository : IGpaTrendRepository
{
    private readonly LuxorLMSAnalyticsDbContext _context;

    public GpaTrendRepository(LuxorLMSAnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GpaTrend>> GetByStudentAsync(Guid studentId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.GpaTrends
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.SemesterNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GpaTrend>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        return await _context.GpaTrends
            .Where(t => t.CourseOfferingId == courseOfferingId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GpaTrend trend, CancellationToken cancellationToken = default)
    {
        await _context.GpaTrends.AddAsync(trend, cancellationToken);
    }

    public void Update(GpaTrend trend)
    {
        _context.GpaTrends.Update(trend);
    }
}

public class GradeDistributionRepository : IGradeDistributionRepository
{
    private readonly LuxorLMSAnalyticsDbContext _context;

    public GradeDistributionRepository(LuxorLMSAnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GradeDistribution>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        return await _context.GradeDistributions
            .Where(d => d.CourseOfferingId == courseOfferingId)
            .OrderBy(d => d.GradeLetter)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GradeDistribution distribution, CancellationToken cancellationToken = default)
    {
        await _context.GradeDistributions.AddAsync(distribution, cancellationToken);
    }

    public void Update(GradeDistribution distribution)
    {
        _context.GradeDistributions.Update(distribution);
    }
}

public class ServerHealthRepository : IServerHealthRepository
{
    private readonly LuxorLMSAnalyticsDbContext _context;

    public ServerHealthRepository(LuxorLMSAnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ServerHealthMetric>> GetRecentAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.ServerHealthMetrics
            .OrderByDescending(m => m.RecordedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ServerHealthMetric metric, CancellationToken cancellationToken = default)
    {
        await _context.ServerHealthMetrics.AddAsync(metric, cancellationToken);
    }
}

public class AnalyticsUnitOfWork : IAnalyticsUnitOfWork
{
    private readonly LuxorLMSAnalyticsDbContext _context;

    public AnalyticsUnitOfWork(LuxorLMSAnalyticsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
