using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface IStudyPlanRepository
{
    Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudyPlan>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByVersionCodeAsync(string versionCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByVersionCodeAsync(string versionCode, Guid excludingId, CancellationToken cancellationToken = default);
    Task AddAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default);
}
