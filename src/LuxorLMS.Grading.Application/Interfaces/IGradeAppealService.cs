using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Interfaces;

public interface IGradeAppealService
{
    Task<Result<IReadOnlyList<GradeAppealDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<GradeAppealDto>>> GetByGradeAsync(Guid studentGradeId, CancellationToken cancellationToken = default);
    Task<Result<GradeAppealDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GradeAppealDto>> CreateAsync(CreateGradeAppealRequest request, Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<GradeAppealDto>> ResolveAsync(Guid id, ResolveGradeAppealRequest request, Guid resolvedBy, CancellationToken cancellationToken = default);
}
