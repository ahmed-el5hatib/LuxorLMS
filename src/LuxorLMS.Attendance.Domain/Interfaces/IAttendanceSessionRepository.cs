using LuxorLMS.Attendance.Domain.Entities;

namespace LuxorLMS.Attendance.Domain.Interfaces;

public interface IAttendanceSessionRepository
{
    Task<AttendanceSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceSession>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceSession>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task AddAsync(AttendanceSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(AttendanceSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(AttendanceSession session, CancellationToken cancellationToken = default);
}
