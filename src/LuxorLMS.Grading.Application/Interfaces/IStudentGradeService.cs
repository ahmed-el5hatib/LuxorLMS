using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Interfaces;

public interface IStudentGradeService
{
    Task<Result<IReadOnlyList<StudentGradeDto>>> GetByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<StudentGradeDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<StudentGradeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<StudentGradeDto>> EnterAsync(EnterGradeRequest request, Guid enteredBy, CancellationToken cancellationToken = default);
    Task<Result<StudentGradeDto>> UpdateAsync(Guid id, UpdateGradeRequest request, CancellationToken cancellationToken = default);

    // Publish workflow: Draft -> PendingDeptHead -> DeptHeadApproved -> Published
    Task<Result<StudentGradeDto>> SubmitForApprovalAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<StudentGradeDto>> DeptHeadApproveAsync(Guid id, bool approve, Guid approvedBy, CancellationToken cancellationToken = default);
    Task<Result<StudentGradeDto>> PublishAsync(Guid id, Guid publishedBy, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
