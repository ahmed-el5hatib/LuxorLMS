using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Identity.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly LuxorLMSIdentityDbContext _context;

    public AuditLogRepository(LuxorLMSIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        return auditLog;
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(al => al.EntityName == entityName && al.EntityId == entityId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .OrderByDescending(al => al.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
