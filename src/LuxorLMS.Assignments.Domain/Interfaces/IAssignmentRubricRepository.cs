using LuxorLMS.Assignments.Domain.Entities;

namespace LuxorLMS.Assignments.Domain.Interfaces;

public interface IAssignmentRubricRepository
{
    Task<AssignmentRubric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentRubric>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task AddAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default);
}
