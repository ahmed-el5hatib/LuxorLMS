using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface ICourseService
{
    Task<Result<IReadOnlyList<CourseDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<Result<CourseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CoursePrerequisiteDto>>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Result<CourseDto>> CreateAsync(CreateCourseRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<CourseDto>> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
