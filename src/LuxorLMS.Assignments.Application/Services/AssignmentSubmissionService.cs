using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Assignments.Application.Interfaces;
using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Enums;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Services;

public class AssignmentSubmissionService : IAssignmentSubmissionService
{
    private readonly IAssignmentSubmissionRepository _submissionRepository;
    private readonly IAssignmentFileRepository _fileRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlagiarismCheckService _plagiarism;

    public AssignmentSubmissionService(
        IAssignmentSubmissionRepository submissionRepository,
        IAssignmentFileRepository fileRepository,
        IAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork,
        IPlagiarismCheckService plagiarism)
    {
        _submissionRepository = submissionRepository;
        _fileRepository = fileRepository;
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
        _plagiarism = plagiarism;
    }

    public async Task<Result<IReadOnlyList<AssignmentSubmissionDto>>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var items = await _submissionRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken);
        return Result<IReadOnlyList<AssignmentSubmissionDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<AssignmentSubmissionDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _submissionRepository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<AssignmentSubmissionDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<AssignmentSubmissionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.NotFound", "Submission not found."));
        return Result<AssignmentSubmissionDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentSubmissionDto>> SubmitAsync(SubmitAssignmentRequest request, Guid submittedBy, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment is null)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.InvalidAssignment", "Referenced assignment does not exist."));

        if (assignment.Status != AssignmentStatus.Published)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.NotOpen", "Assignment is not open for submissions."));

        var existing = await _submissionRepository.GetByAssignmentAndStudentAsync(request.AssignmentId, request.StudentId, cancellationToken);
        if (existing is not null)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.Duplicate", "A submission already exists for this student."));

        var now = DateTime.UtcNow;
        var status = SubmissionStatus.Submitted;
        if (now > assignment.DueDate && !assignment.AllowLateSubmission)
            status = SubmissionStatus.Late;

        var entity = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            SubmittedAt = now,
            Score = null,
            Feedback = null,
            Status = status,
            PlagiarismReportUrl = null,
            PlagiarismScore = null,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = submittedBy
        };

        await _submissionRepository.AddAsync(entity, cancellationToken);

        var report = await _plagiarism.CheckAsync(entity.Id, cancellationToken);
        if (report.IsSuccess && report.Value is not null)
        {
            entity.PlagiarismScore = report.Value.PlagiarismScore;
            entity.PlagiarismReportUrl = report.Value.ReportUrl;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentSubmissionDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentSubmissionDto>> GradeAsync(Guid id, GradeSubmissionRequest request, Guid gradedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.NotFound", "Submission not found."));

        var assignment = await _assignmentRepository.GetByIdAsync(entity.AssignmentId, cancellationToken);
        if (assignment is null)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.InvalidAssignment", "Referenced assignment does not exist."));

        if (request.Score is < 0)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.InvalidScore", "Score must be non-negative."));

        if (request.Score > assignment.MaxScore)
            return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.ScoreExceedsMax", "Score cannot exceed the assignment max score."));

        entity.Score = request.Score;
        entity.Feedback = request.Feedback;
        entity.Status = SubmissionStatus.Graded;

        await _submissionRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentSubmissionDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentSubmissionDto>> ReturnAsync(Guid id, ReturnSubmissionRequest request, Guid returnedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentSubmissionDto>.Failure(new Error("Submission.NotFound", "Submission not found."));

        entity.Feedback = request.Feedback;
        entity.Status = SubmissionStatus.Returned;

        await _submissionRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentSubmissionDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Submission.NotFound", "Submission not found."));

        if (entity.Status == SubmissionStatus.Graded)
            return Result.Failure(new Error("Submission.NotDeletable", "Graded submissions cannot be deleted."));

        await _submissionRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<AssignmentFileDto>>> GetFilesAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var items = await _fileRepository.GetBySubmissionIdAsync(submissionId, cancellationToken);
        return Result<IReadOnlyList<AssignmentFileDto>>.Success(items.Select(MapFile).ToList());
    }

    public async Task<Result<AssignmentFileDto>> AddFileAsync(AddAssignmentFileRequest request, Guid uploadedBy, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepository.GetByIdAsync(request.AssignmentSubmissionId, cancellationToken);
        if (submission is null)
            return Result<AssignmentFileDto>.Failure(new Error("File.InvalidSubmission", "Referenced submission does not exist."));

        if (submission.Status == SubmissionStatus.Graded)
            return Result<AssignmentFileDto>.Failure(new Error("File.SubmissionGraded", "Cannot add files to a graded submission."));

        var version = await _fileRepository.GetNextVersionAsync(request.AssignmentSubmissionId, cancellationToken);
        var now = DateTime.UtcNow;

        var entity = new AssignmentFile
        {
            Id = Guid.NewGuid(),
            AssignmentSubmissionId = request.AssignmentSubmissionId,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            Version = version,
            UploadedAt = now,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = uploadedBy
        };

        await _fileRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentFileDto>.Success(MapFile(entity));
    }

    public async Task<Result> DeleteFileAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var entity = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (entity is null) return Result.Failure(new Error("File.NotFound", "File not found."));

        await _fileRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static AssignmentSubmissionDto Map(AssignmentSubmission s) => new(
        s.Id, s.AssignmentId, s.StudentId, s.SubmittedAt, s.Score, s.Feedback, s.Status,
        s.PlagiarismReportUrl, s.PlagiarismScore, s.IsActive, s.CreatedAt, s.CreatedBy);

    private static AssignmentFileDto MapFile(AssignmentFile f) => new(
        f.Id, f.AssignmentSubmissionId, f.FileName, f.FileUrl, f.ContentType, f.FileSizeBytes,
        f.Version, f.UploadedAt, f.IsActive, f.CreatedAt, f.CreatedBy);
}
