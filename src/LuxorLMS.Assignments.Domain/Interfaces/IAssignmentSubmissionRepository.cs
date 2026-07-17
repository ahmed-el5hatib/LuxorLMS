using LuxorLMS.Assignments.Domain.Entities;

namespace LuxorLMS.Assignments.Domain.Interfaces;

public interface IAssignmentSubmissionRepository
{
    Task<AssignmentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentSubmission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<AssignmentSubmission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default);
}
