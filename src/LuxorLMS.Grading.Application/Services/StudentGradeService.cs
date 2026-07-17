using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Enums;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Services;

public class StudentGradeService : IStudentGradeService
{
    private const int AppealWindowDays = 7;

    private readonly IStudentGradeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGradeScaleService _scale;
    private readonly IAcademicGradingGateway _academic;
    private readonly IGpaService _gpaService;

    public StudentGradeService(
        IStudentGradeRepository repository,
        IUnitOfWork unitOfWork,
        IGradeScaleService scale,
        IAcademicGradingGateway academic,
        IGpaService gpaService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _scale = scale;
        _academic = academic;
        _gpaService = gpaService;
    }

    public async Task<Result<IReadOnlyList<StudentGradeDto>>> GetByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<StudentGradeDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<StudentGradeDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<StudentGradeDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<StudentGradeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentGradeDto>.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));
        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result<StudentGradeDto>> EnterAsync(EnterGradeRequest request, Guid enteredBy, CancellationToken cancellationToken = default)
    {
        if (request.RawScore is < 0 or > 100)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidScore", "Raw score must be between 0 and 100."));

        var course = await _academic.GetCourseAsync(request.CourseId, cancellationToken);
        if (course is null)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidCourse", "Referenced course does not exist."));

        var existing = await _repository.GetByOfferingAndStudentAsync(request.CourseOfferingId, request.StudentId, cancellationToken);
        if (existing is not null)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.Duplicate", "A grade already exists for this student in this offering."));

        var letter = _scale.ToLetter(request.RawScore);
        var entity = new StudentGrade
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            SemesterId = request.SemesterId,
            CreditHours = course.CreditHours,
            RawScore = request.RawScore,
            GradeLetter = letter,
            GradePoints = _scale.ToGradePoints(letter),
            PublishStatus = GradePublishStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = enteredBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result<StudentGradeDto>> UpdateAsync(Guid id, UpdateGradeRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentGradeDto>.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));

        if (entity.PublishStatus == GradePublishStatus.Published)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.AlreadyPublished", "Published grades cannot be edited directly; use the appeal process."));

        if (request.RawScore is < 0 or > 100)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidScore", "Raw score must be between 0 and 100."));

        var letter = _scale.ToLetter(request.RawScore);
        entity.RawScore = request.RawScore;
        entity.GradeLetter = letter;
        entity.GradePoints = _scale.ToGradePoints(letter);
        // Editing resets an in-review grade back to draft.
        entity.PublishStatus = GradePublishStatus.Draft;
        entity.DeptHeadApprovedBy = null;
        entity.DeptHeadApprovedAt = null;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result<StudentGradeDto>> SubmitForApprovalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentGradeDto>.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));

        if (entity.PublishStatus != GradePublishStatus.Draft && entity.PublishStatus != GradePublishStatus.Rejected)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidTransition", "Only draft or rejected grades can be submitted for approval."));

        entity.PublishStatus = GradePublishStatus.PendingDeptHead;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result<StudentGradeDto>> DeptHeadApproveAsync(Guid id, bool approve, Guid approvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentGradeDto>.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));

        if (entity.PublishStatus != GradePublishStatus.PendingDeptHead)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidTransition", "Grade is not pending department-head approval."));

        entity.PublishStatus = approve ? GradePublishStatus.DeptHeadApproved : GradePublishStatus.Rejected;
        entity.DeptHeadApprovedBy = approve ? approvedBy : null;
        entity.DeptHeadApprovedAt = approve ? DateTime.UtcNow : null;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result<StudentGradeDto>> PublishAsync(Guid id, Guid publishedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentGradeDto>.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));

        if (entity.PublishStatus != GradePublishStatus.DeptHeadApproved)
            return Result<StudentGradeDto>.Failure(new Error("StudentGrade.InvalidTransition", "Grade must be department-head approved before publishing."));

        var now = DateTime.UtcNow;
        entity.PublishStatus = GradePublishStatus.Published;
        entity.PublishedBy = publishedBy;
        entity.PublishedAt = now;
        entity.AppealDeadline = now.AddDays(AppealWindowDays);

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Recompute and persist CGPA now that a new grade is published.
        await _gpaService.RecalculateAndPersistCgpaAsync(entity.StudentId, cancellationToken);

        return Result<StudentGradeDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("StudentGrade.NotFound", "Student grade not found."));

        if (entity.PublishStatus == GradePublishStatus.Published)
            return Result.Failure(new Error("StudentGrade.AlreadyPublished", "Published grades cannot be deleted."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static StudentGradeDto Map(StudentGrade g) => new(
        g.Id, g.CourseOfferingId, g.StudentId, g.CourseId, g.SemesterId, g.CreditHours,
        g.RawScore, g.GradeLetter, g.GradePoints, g.PublishStatus,
        g.DeptHeadApprovedBy, g.DeptHeadApprovedAt, g.PublishedBy, g.PublishedAt, g.AppealDeadline, g.CreatedAt);
}
