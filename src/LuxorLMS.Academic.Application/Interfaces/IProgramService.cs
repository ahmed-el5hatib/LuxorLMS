using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IProgramService
{
    Task<Result<IReadOnlyList<ProgramDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<Result<ProgramDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ProgramDto>> CreateAsync(CreateProgramRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<ProgramDto>> UpdateAsync(Guid id, UpdateProgramRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
