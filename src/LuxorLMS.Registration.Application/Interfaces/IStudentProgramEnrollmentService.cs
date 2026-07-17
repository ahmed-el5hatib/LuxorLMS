using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.DTOs;

namespace LuxorLMS.Registration.Application.Interfaces;

public interface IStudentProgramEnrollmentService
{
    Task<Result<IReadOnlyList<StudentProgramEnrollmentDto>>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<StudentProgramEnrollmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<StudentProgramEnrollmentDto>> CreateAsync(CreateStudentProgramEnrollmentRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<StudentProgramEnrollmentDto>> UpdateAsync(Guid id, UpdateStudentProgramEnrollmentRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
