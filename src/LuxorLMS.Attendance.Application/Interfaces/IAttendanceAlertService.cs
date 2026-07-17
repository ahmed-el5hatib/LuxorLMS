using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Interfaces;

public interface IAttendanceAlertService
{
    Task<Result<IReadOnlyList<AttendanceAlertDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AttendanceAlertDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
}
