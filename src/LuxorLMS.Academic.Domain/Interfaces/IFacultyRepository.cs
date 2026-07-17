using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface IFacultyRepository
{
    Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Faculty>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid excludingId, CancellationToken cancellationToken = default);
    Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default);
    Task UpdateAsync(Faculty faculty, CancellationToken cancellationToken = default);
    Task DeleteAsync(Faculty faculty, CancellationToken cancellationToken = default);
}
