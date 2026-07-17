using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Interfaces;

public interface IAttendanceSessionService
{
    Task<Result<IReadOnlyList<AttendanceSessionDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AttendanceSessionDto>>> GetBySectionAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<Result<AttendanceSessionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AttendanceSessionDto>> CreateAsync(CreateAttendanceSessionRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<AttendanceSessionDto>> UpdateAsync(Guid id, UpdateAttendanceSessionRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
