using LuxorLMS.Forums.Application.Interfaces;
using LuxorLMS.Identity.Infrastructure.Services;

namespace LuxorLMS.Forums.Infrastructure.Services;

public class ForumsAuditLogger : IForumsAuditLogger
{
    private readonly IAuditLogger _auditLogger;

    public ForumsAuditLogger(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public async Task LogAsync(Guid userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default)
    {
        await _auditLogger.LogAsync(userId, action, entityName, entityId, oldValues, newValues, cancellationToken: cancellationToken);
    }
}
