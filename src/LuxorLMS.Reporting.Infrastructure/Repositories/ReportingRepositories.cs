using LuxorLMS.Reporting.Domain.Entities;
using LuxorLMS.Reporting.Domain.Enums;
using LuxorLMS.Reporting.Domain.Interfaces;
using LuxorLMS.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Reporting.Infrastructure.Repositories;

public class ReportJobRepository : IReportJobRepository
{
    private readonly LuxorLMSReportingDbContext _context;

    public ReportJobRepository(LuxorLMSReportingDbContext context)
    {
        _context = context;
    }

    public async Task<ReportJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportJobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportJob>> GetByUserAsync(Guid userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.ReportJobs
            .Where(j => j.RequestedBy == userId)
            .OrderByDescending(j => j.RequestedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportJob>> GetPendingAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await _context.ReportJobs
            .Where(j => j.Status == ReportStatus.Queued || j.Status == ReportStatus.Processing)
            .OrderBy(j => j.RequestedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ReportJob job, CancellationToken cancellationToken = default)
    {
        await _context.ReportJobs.AddAsync(job, cancellationToken);
    }

    public void Update(ReportJob job)
    {
        _context.ReportJobs.Update(job);
    }
}

public class ReportTemplateRepository : IReportTemplateRepository
{
    private readonly LuxorLMSReportingDbContext _context;

    public ReportTemplateRepository(LuxorLMSReportingDbContext context)
    {
        _context = context;
    }

    public async Task<ReportTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates.FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public async Task<ReportTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ReportTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.ReportTemplates.AddAsync(template, cancellationToken);
    }

    public void Update(ReportTemplate template)
    {
        _context.ReportTemplates.Update(template);
    }
}

public class ReportingUnitOfWork : IReportingUnitOfWork
{
    private readonly LuxorLMSReportingDbContext _context;

    public ReportingUnitOfWork(LuxorLMSReportingDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
