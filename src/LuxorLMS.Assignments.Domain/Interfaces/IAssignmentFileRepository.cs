using LuxorLMS.Assignments.Domain.Entities;

namespace LuxorLMS.Assignments.Domain.Interfaces;

public interface IAssignmentFileRepository
{
    Task<AssignmentFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentFile>> GetBySubmissionIdAsync(Guid assignmentSubmissionId, CancellationToken cancellationToken = default);
    Task<int> GetNextVersionAsync(Guid assignmentSubmissionId, CancellationToken cancellationToken = default);
    Task AddAsync(AssignmentFile file, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssignmentFile file, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssignmentFile file, CancellationToken cancellationToken = default);
}
