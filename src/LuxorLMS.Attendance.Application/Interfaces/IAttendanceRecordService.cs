using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Interfaces;

public interface IAttendanceRecordService
{
    Task<Result<AttendanceRecordDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetBySessionAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<AttendanceRecordDto>> MarkAsync(MarkAttendanceRequest request, Guid markedBy, CancellationToken cancellationToken = default);
}
