using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface ICourseOfferingRepository
{
    Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseOffering>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<int> CountEnrolledAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(CourseOffering offering, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseOffering offering, CancellationToken cancellationToken = default);
    Task DeleteAsync(CourseOffering offering, CancellationToken cancellationToken = default);
}
