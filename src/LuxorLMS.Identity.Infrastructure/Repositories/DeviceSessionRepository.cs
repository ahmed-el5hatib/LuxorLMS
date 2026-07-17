using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Identity.Infrastructure.Repositories;

public class DeviceSessionRepository : IDeviceSessionRepository
{
    private readonly LuxorLMSIdentityDbContext _context;

    public DeviceSessionRepository(LuxorLMSIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DeviceSessions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<DeviceSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.DeviceSessions
            .Where(ds => ds.UserId == userId)
            .OrderByDescending(ds => ds.LastActiveAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DeviceSession session, CancellationToken cancellationToken = default)
    {
        await _context.DeviceSessions.AddAsync(session, cancellationToken);
    }

    public Task UpdateAsync(DeviceSession session, CancellationToken cancellationToken = default)
    {
        _context.DeviceSessions.Update(session);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _context.DeviceSessions.FindAsync(new object[] { id }, cancellationToken);
        if (session != null)
        {
            _context.DeviceSessions.Remove(session);
        }
    }
}
