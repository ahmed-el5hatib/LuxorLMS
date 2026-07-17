using LuxorLMS.Grading.Domain.Entities;

namespace LuxorLMS.Grading.Domain.Interfaces;

public interface IGradeAppealRepository
{
    Task<GradeAppeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GradeAppeal>> GetByStudentGradeIdAsync(Guid studentGradeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GradeAppeal>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(GradeAppeal appeal, CancellationToken cancellationToken = default);
    Task UpdateAsync(GradeAppeal appeal, CancellationToken cancellationToken = default);
    Task DeleteAsync(GradeAppeal appeal, CancellationToken cancellationToken = default);
}
