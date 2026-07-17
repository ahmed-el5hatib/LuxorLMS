using LuxorLMS.Analytics.Domain.Entities;
using LuxorLMS.Analytics.Domain.Enums;

namespace LuxorLMS.Analytics.Domain.Interfaces;

public interface IAnalyticsKpiRepository
{
    Task<AnalyticsKpi?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnalyticsKpi>> GetByFilterAsync(Guid? courseOfferingId, Guid? departmentId, Guid? programId, MetricType? metricType, TimeRange? timeRange, DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task AddAsync(AnalyticsKpi kpi, CancellationToken cancellationToken = default);
    void Update(AnalyticsKpi kpi);
}

public interface IGpaTrendRepository
{
    Task<IReadOnlyList<GpaTrend>> GetByStudentAsync(Guid studentId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GpaTrend>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(GpaTrend trend, CancellationToken cancellationToken = default);
    void Update(GpaTrend trend);
}

public interface IGradeDistributionRepository
{
    Task<IReadOnlyList<GradeDistribution>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(GradeDistribution distribution, CancellationToken cancellationToken = default);
    void Update(GradeDistribution distribution);
}

public interface IServerHealthRepository
{
    Task<IReadOnlyList<ServerHealthMetric>> GetRecentAsync(int limit = 100, CancellationToken cancellationToken = default);
    Task AddAsync(ServerHealthMetric metric, CancellationToken cancellationToken = default);
}

public interface IAnalyticsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
