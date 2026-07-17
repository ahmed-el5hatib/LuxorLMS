using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.DTOs;

namespace LuxorLMS.Registration.Application.Interfaces;

public interface ICourseEnrollmentService
{
    Task<Result<IReadOnlyList<CourseEnrollmentDto>>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CourseEnrollmentDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<Result<CourseEnrollmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CourseEnrollmentDto>> RegisterAsync(RegisterCourseRequest request, Guid requestedBy, CancellationToken cancellationToken = default);
    Task<Result<CourseEnrollmentDto>> ApproveAsync(Guid id, bool approve, Guid approvedBy, CancellationToken cancellationToken = default);
    Task<Result<CourseEnrollmentDto>> WithdrawAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
