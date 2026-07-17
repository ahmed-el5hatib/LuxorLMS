using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IDepartmentService
{
    Task<Result<IReadOnlyList<DepartmentDto>>> GetByFacultyIdAsync(Guid facultyId, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
