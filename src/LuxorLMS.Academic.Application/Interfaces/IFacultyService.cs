using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IFacultyService
{
    Task<Result<IReadOnlyList<FacultyDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<FacultyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<FacultyDto>> CreateAsync(CreateFacultyRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<FacultyDto>> UpdateAsync(Guid id, UpdateFacultyRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
