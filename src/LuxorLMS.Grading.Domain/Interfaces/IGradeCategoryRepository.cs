using LuxorLMS.Grading.Domain.Entities;

namespace LuxorLMS.Grading.Domain.Interfaces;

public interface IGradeCategoryRepository
{
    Task<GradeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GradeCategory>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(GradeCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(GradeCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(GradeCategory category, CancellationToken cancellationToken = default);
}
