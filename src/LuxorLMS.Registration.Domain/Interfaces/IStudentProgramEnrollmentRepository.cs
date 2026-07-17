using LuxorLMS.Registration.Domain.Entities;

namespace LuxorLMS.Registration.Domain.Interfaces;

public interface IStudentProgramEnrollmentRepository
{
    Task<StudentProgramEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentProgramEnrollment?> GetActiveByStudentAndProgramAsync(Guid studentId, Guid programId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentProgramEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default);
}
