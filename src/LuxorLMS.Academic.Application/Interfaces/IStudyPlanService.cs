using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IStudyPlanService
{
    Task<Result<IReadOnlyList<StudyPlanDto>>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<Result<StudyPlanDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<StudyPlanDto>> CreateAsync(CreateStudyPlanRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<StudyPlanDto>> UpdateAsync(Guid id, UpdateStudyPlanRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
