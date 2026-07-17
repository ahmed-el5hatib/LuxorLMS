using LuxorLMS.Analytics.Application.DTOs;
using LuxorLMS.Analytics.Domain.Entities;
using LuxorLMS.Analytics.Domain.Enums;
using LuxorLMS.Kernel;

namespace LuxorLMS.Analytics.Application.Interfaces;

public interface IAnalyticsService
{
    Task<Result<PagedResult<AnalyticsKpiDto>>> GetKpisAsync(AnalyticsFilterRequest filter, CancellationToken cancellationToken = default);
    Task<Result<AnalyticsKpiDto>> GetLatestKpiAsync(string key, Guid? courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<GpaTrendDto>>> GetStudentGpaTrendAsync(Guid studentId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<GradeDistributionDto>>> GetCourseGradeDistributionAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ServerHealthMetricDto>>> GetServerHealthAsync(int limit = 100, CancellationToken cancellationToken = default);
    Task<Result<AnalyticsKpiDto>> RecordServerHealthAsync(string metricName, decimal value, string? unit, string status, string? details, CancellationToken cancellationToken = default);
}
