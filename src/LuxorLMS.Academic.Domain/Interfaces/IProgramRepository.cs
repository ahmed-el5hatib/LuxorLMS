using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface IProgramRepository
{
    Task<Program?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Program>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid excludingId, CancellationToken cancellationToken = default);
    Task AddAsync(Program program, CancellationToken cancellationToken = default);
    Task UpdateAsync(Program program, CancellationToken cancellationToken = default);
    Task DeleteAsync(Program program, CancellationToken cancellationToken = default);
}
