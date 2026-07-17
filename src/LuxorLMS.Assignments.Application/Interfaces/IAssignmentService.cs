using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Interfaces;

public interface IAssignmentService
{
    Task<Result<IReadOnlyList<AssignmentDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AssignmentDto>>> GetActiveByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<AssignmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<AssignmentDto>> CreateAsync(CreateAssignmentRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<AssignmentDto>> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssignmentDto>> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AssignmentDto>> CloseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AssignmentDto>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AssignmentRubricDto>>> GetRubricAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<Result<AssignmentRubricDto>> AddRubricAsync(CreateAssignmentRubricRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<AssignmentRubricDto>> UpdateRubricAsync(Guid id, UpdateAssignmentRubricRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRubricAsync(Guid id, CancellationToken cancellationToken = default);
}
