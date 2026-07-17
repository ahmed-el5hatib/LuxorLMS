using LuxorLMS.Attendance.Domain.Entities;

namespace LuxorLMS.Attendance.Domain.Interfaces;

public interface IAttendanceRecordRepository
{
    Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> GetBySessionAndStudentAsync(Guid attendanceSessionId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetBySessionIdAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    Task DeleteAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
}
