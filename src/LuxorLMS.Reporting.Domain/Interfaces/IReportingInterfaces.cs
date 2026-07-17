using LuxorLMS.Reporting.Domain.Entities;

namespace LuxorLMS.Reporting.Domain.Interfaces;

public interface IReportJobRepository
{
    Task<ReportJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportJob>> GetByUserAsync(Guid userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportJob>> GetPendingAsync(int limit = 50, CancellationToken cancellationToken = default);
    Task AddAsync(ReportJob job, CancellationToken cancellationToken = default);
    void Update(ReportJob job);
}

public interface IReportTemplateRepository
{
    Task<ReportTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ReportTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ReportTemplate template, CancellationToken cancellationToken = default);
    void Update(ReportTemplate template);
}

public interface IReportingUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
