using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface ISemesterRepository
{
    Task<Semester?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetByAcademicYearIdAsync(Guid academicYearId, CancellationToken cancellationToken = default);
    Task AddAsync(Semester semester, CancellationToken cancellationToken = default);
    Task UpdateAsync(Semester semester, CancellationToken cancellationToken = default);
    Task DeleteAsync(Semester semester, CancellationToken cancellationToken = default);
}
