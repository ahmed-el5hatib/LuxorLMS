using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface ISemesterService
{
    Task<Result<IReadOnlyList<SemesterDto>>> GetByAcademicYearIdAsync(Guid academicYearId, CancellationToken cancellationToken = default);
    Task<Result<SemesterDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SemesterDto>> CreateAsync(CreateSemesterRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
