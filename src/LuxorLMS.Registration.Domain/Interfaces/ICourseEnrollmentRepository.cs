using LuxorLMS.Registration.Domain.Entities;

namespace LuxorLMS.Registration.Domain.Interfaces;

public interface ICourseEnrollmentRepository
{
    Task<CourseEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CourseEnrollment?> GetByStudentCourseSemesterAsync(Guid studentId, Guid courseId, Guid semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseEnrollment>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseEnrollment>> GetApprovedByStudentAndSemesterAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default);
    Task<int> SumApprovedCreditHoursAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetCompletedCourseIdsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);
}
