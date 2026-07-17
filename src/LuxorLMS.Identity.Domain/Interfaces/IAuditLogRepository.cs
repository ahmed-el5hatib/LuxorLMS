using LuxorLMS.Identity.Domain.Entities;

namespace LuxorLMS.Identity.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
