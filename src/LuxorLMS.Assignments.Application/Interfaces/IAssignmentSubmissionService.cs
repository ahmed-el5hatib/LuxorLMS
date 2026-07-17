using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Interfaces;

public interface IAssignmentSubmissionService
{
    Task<Result<IReadOnlyList<AssignmentSubmissionDto>>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AssignmentSubmissionDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<AssignmentSubmissionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<AssignmentSubmissionDto>> SubmitAsync(SubmitAssignmentRequest request, Guid submittedBy, CancellationToken cancellationToken = default);
    Task<Result<AssignmentSubmissionDto>> GradeAsync(Guid id, GradeSubmissionRequest request, Guid gradedBy, CancellationToken cancellationToken = default);
    Task<Result<AssignmentSubmissionDto>> ReturnAsync(Guid id, ReturnSubmissionRequest request, Guid returnedBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AssignmentFileDto>>> GetFilesAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<Result<AssignmentFileDto>> AddFileAsync(AddAssignmentFileRequest request, Guid uploadedBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteFileAsync(Guid fileId, CancellationToken cancellationToken = default);
}
