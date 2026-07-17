using LuxorLMS.Attendance.Application.DTOs;

namespace LuxorLMS.Attendance.Application.Interfaces;

public interface IAcademicAttendanceGateway
{
    Task<bool> CourseOfferingExistsAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<bool> SectionExistsAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<bool> StudentExistsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetEnrolledStudentIdsAsync(Guid courseOfferingId, Guid? sectionId, CancellationToken cancellationToken = default);
}
