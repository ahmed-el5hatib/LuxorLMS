using LuxorLMS.Attendance.Domain.Entities;

namespace LuxorLMS.Attendance.Domain.Interfaces;

public interface IAttendanceAlertRepository
{
    Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetBySessionIdAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default);
}
