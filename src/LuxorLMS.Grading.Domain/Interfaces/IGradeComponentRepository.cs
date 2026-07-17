using LuxorLMS.Grading.Domain.Entities;

namespace LuxorLMS.Grading.Domain.Interfaces;

public interface IGradeComponentRepository
{
    Task<GradeComponent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GradeComponent>> GetByCategoryIdAsync(Guid gradeCategoryId, CancellationToken cancellationToken = default);
    Task AddAsync(GradeComponent component, CancellationToken cancellationToken = default);
    Task UpdateAsync(GradeComponent component, CancellationToken cancellationToken = default);
    Task DeleteAsync(GradeComponent component, CancellationToken cancellationToken = default);
}
