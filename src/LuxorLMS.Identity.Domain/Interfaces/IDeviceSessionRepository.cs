using LuxorLMS.Identity.Domain.Entities;

namespace LuxorLMS.Identity.Domain.Interfaces;

public interface IDeviceSessionRepository
{
    Task<DeviceSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeviceSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(DeviceSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeviceSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
