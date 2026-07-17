using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string courseCode, Guid excludingId, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CoursePrerequisite>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task DeleteAsync(Course course, CancellationToken cancellationToken = default);
}
