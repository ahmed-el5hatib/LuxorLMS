using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditController(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(logs);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogRepository.GetByUserIdAsync(userId, pageNumber, pageSize, cancellationToken);
        return Ok(logs);
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<IActionResult> GetByEntity(string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogRepository.GetByEntityAsync(entityName, entityId, cancellationToken);
        return Ok(logs);
    }
}
