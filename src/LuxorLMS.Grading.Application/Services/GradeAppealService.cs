using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Enums;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Services;

public class GradeAppealService : IGradeAppealService
{
    private readonly IGradeAppealRepository _repository;
    private readonly IStudentGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GradeAppealService(
        IGradeAppealRepository repository,
        IStudentGradeRepository gradeRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<GradeAppealDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<GradeAppealDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<GradeAppealDto>>> GetByGradeAsync(Guid studentGradeId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentGradeIdAsync(studentGradeId, cancellationToken);
        return Result<IReadOnlyList<GradeAppealDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<GradeAppealDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.NotFound", "Grade appeal not found."));
        return Result<GradeAppealDto>.Success(Map(entity));
    }

    public async Task<Result<GradeAppealDto>> CreateAsync(CreateGradeAppealRequest request, Guid studentId, CancellationToken cancellationToken = default)
    {
        var grade = await _gradeRepository.GetByIdAsync(request.StudentGradeId, cancellationToken);
        if (grade is null) return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.InvalidGrade", "Referenced grade does not exist."));

        if (grade.StudentId != studentId)
            return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.Forbidden", "You may only appeal your own grades."));

        if (grade.PublishStatus != GradePublishStatus.Published)
            return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.NotPublished", "Only published grades can be appealed."));

        if (grade.AppealDeadline is null || DateTime.UtcNow > grade.AppealDeadline.Value)
            return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.WindowClosed", "The 7-day appeal window has closed."));

        var existing = await _repository.GetByStudentGradeIdAsync(request.StudentGradeId, cancellationToken);
        if (existing.Any(a => a.Status == AppealStatus.Open))
            return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.Duplicate", "An open appeal already exists for this grade."));

        var entity = new GradeAppeal
        {
            Id = Guid.NewGuid(),
            StudentGradeId = request.StudentGradeId,
            StudentId = studentId,
            Reason = request.Reason,
            Status = AppealStatus.Open,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = studentId
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeAppealDto>.Success(Map(entity));
    }

    public async Task<Result<GradeAppealDto>> ResolveAsync(Guid id, ResolveGradeAppealRequest request, Guid resolvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.NotFound", "Grade appeal not found."));

        if (entity.Status != AppealStatus.Open)
            return Result<GradeAppealDto>.Failure(new Error("GradeAppeal.AlreadyResolved", "This appeal has already been resolved."));

        entity.Status = request.Approve ? AppealStatus.Approved : AppealStatus.Rejected;
        entity.Resolution = request.Resolution;
        entity.ResolvedBy = resolvedBy;
        entity.ResolvedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeAppealDto>.Success(Map(entity));
    }

    private static GradeAppealDto Map(GradeAppeal a) => new(
        a.Id, a.StudentGradeId, a.StudentId, a.Reason, a.Status, a.Resolution, a.ResolvedBy, a.ResolvedAt, a.CreatedAt);
}
