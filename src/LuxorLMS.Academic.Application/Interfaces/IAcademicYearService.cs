using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IAcademicYearService
{
    Task<Result<IReadOnlyList<AcademicYearDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AcademicYearDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AcademicYearDto>> CreateAsync(CreateAcademicYearRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
