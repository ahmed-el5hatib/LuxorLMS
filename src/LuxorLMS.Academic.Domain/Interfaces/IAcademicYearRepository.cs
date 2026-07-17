using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface IAcademicYearRepository
{
    Task<AcademicYear?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademicYear>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByLabelAsync(string label, CancellationToken cancellationToken = default);
    Task<bool> ExistsByLabelAsync(string label, Guid excludingId, CancellationToken cancellationToken = default);
    Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task UpdateAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task DeleteAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
}
