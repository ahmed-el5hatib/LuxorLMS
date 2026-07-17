using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Assignments.Application.Interfaces;
using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Enums;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IAssignmentRubricRepository _rubricRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicAssignmentGateway _academic;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IAssignmentRubricRepository rubricRepository,
        IUnitOfWork unitOfWork,
        IAcademicAssignmentGateway academic)
    {
        _assignmentRepository = assignmentRepository;
        _rubricRepository = rubricRepository;
        _unitOfWork = unitOfWork;
        _academic = academic;
    }

    public async Task<Result<IReadOnlyList<AssignmentDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _assignmentRepository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<AssignmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<AssignmentDto>>> GetActiveByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _assignmentRepository.GetActiveByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<AssignmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<AssignmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentDto>.Failure(new Error("Assignment.NotFound", "Assignment not found."));
        return Result<AssignmentDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentDto>> CreateAsync(CreateAssignmentRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var offering = await _academic.GetCourseOfferingAsync(request.CourseOfferingId, cancellationToken);
        if (offering is null)
            return Result<AssignmentDto>.Failure(new Error("Assignment.InvalidOffering", "Referenced course offering does not exist."));

        if (request.MaxScore < 0)
            return Result<AssignmentDto>.Failure(new Error("Assignment.InvalidScore", "Max score must be non-negative."));

        var entity = new Assignment
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            MaxScore = request.MaxScore,
            AllowLateSubmission = request.AllowLateSubmission,
            Status = AssignmentStatus.Draft,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _assignmentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentDto>> UpdateAsync(Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentDto>.Failure(new Error("Assignment.NotFound", "Assignment not found."));

        if (entity.Status == AssignmentStatus.Archived)
            return Result<AssignmentDto>.Failure(new Error("Assignment.Archived", "Archived assignments cannot be edited."));

        if (request.MaxScore < 0)
            return Result<AssignmentDto>.Failure(new Error("Assignment.InvalidScore", "Max score must be non-negative."));

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.DueDate = request.DueDate;
        entity.MaxScore = request.MaxScore;
        entity.AllowLateSubmission = request.AllowLateSubmission;

        await _assignmentRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentDto>.Success(Map(entity));
    }

    public async Task<Result<AssignmentDto>> PublishAsync(Guid id, CancellationToken cancellationToken = default)
        => await TransitionStatusAsync(id, AssignmentStatus.Published, "Assignment.AlreadyPublished", "Only draft assignments can be published.", cancellationToken);

    public async Task<Result<AssignmentDto>> CloseAsync(Guid id, CancellationToken cancellationToken = default)
        => await TransitionStatusAsync(id, AssignmentStatus.Closed, "Assignment.AlreadyClosed", "Only published assignments can be closed.", cancellationToken);

    public async Task<Result<AssignmentDto>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
        => await TransitionStatusAsync(id, AssignmentStatus.Archived, "Assignment.AlreadyArchived", "Only closed assignments can be archived.", cancellationToken);

    private async Task<Result<AssignmentDto>> TransitionStatusAsync(
        Guid id, AssignmentStatus target, string errorCode, string errorMessage, CancellationToken cancellationToken)
    {
        var entity = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentDto>.Failure(new Error("Assignment.NotFound", "Assignment not found."));

        var allowed = target switch
        {
            AssignmentStatus.Published => entity.Status == AssignmentStatus.Draft,
            AssignmentStatus.Closed => entity.Status == AssignmentStatus.Published,
            AssignmentStatus.Archived => entity.Status == AssignmentStatus.Closed,
            _ => false
        };

        if (!allowed)
            return Result<AssignmentDto>.Failure(new Error(errorCode, errorMessage));

        entity.Status = target;
        await _assignmentRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Assignment.NotFound", "Assignment not found."));

        if (entity.Status == AssignmentStatus.Published || entity.Status == AssignmentStatus.Closed)
            return Result.Failure(new Error("Assignment.NotDeletable", "Published or closed assignments cannot be deleted."));

        await _assignmentRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<AssignmentRubricDto>>> GetRubricAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var items = await _rubricRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken);
        return Result<IReadOnlyList<AssignmentRubricDto>>.Success(items.OrderBy(r => r.DisplayOrder).Select(MapRubric).ToList());
    }

    public async Task<Result<AssignmentRubricDto>> AddRubricAsync(CreateAssignmentRubricRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment is null)
            return Result<AssignmentRubricDto>.Failure(new Error("Assignment.NotFound", "Referenced assignment does not exist."));

        if (request.MaxPoints <= 0)
            return Result<AssignmentRubricDto>.Failure(new Error("Rubric.InvalidPoints", "Max points must be greater than zero."));

        var entity = new AssignmentRubric
        {
            Id = Guid.NewGuid(),
            AssignmentId = request.AssignmentId,
            Criteria = request.Criteria,
            MaxPoints = request.MaxPoints,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _rubricRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentRubricDto>.Success(MapRubric(entity));
    }

    public async Task<Result<AssignmentRubricDto>> UpdateRubricAsync(Guid id, UpdateAssignmentRubricRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _rubricRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AssignmentRubricDto>.Failure(new Error("Rubric.NotFound", "Rubric criterion not found."));

        if (request.MaxPoints <= 0)
            return Result<AssignmentRubricDto>.Failure(new Error("Rubric.InvalidPoints", "Max points must be greater than zero."));

        entity.Criteria = request.Criteria;
        entity.MaxPoints = request.MaxPoints;
        entity.Description = request.Description;
        entity.DisplayOrder = request.DisplayOrder;

        await _rubricRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssignmentRubricDto>.Success(MapRubric(entity));
    }

    public async Task<Result> DeleteRubricAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _rubricRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Rubric.NotFound", "Rubric criterion not found."));

        await _rubricRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static AssignmentDto Map(Assignment a) => new(
        a.Id, a.CourseOfferingId, a.Title, a.Description, a.DueDate, a.MaxScore,
        a.AllowLateSubmission, a.Status, a.IsActive, a.CreatedAt, a.CreatedBy);

    private static AssignmentRubricDto MapRubric(AssignmentRubric r) => new(
        r.Id, r.AssignmentId, r.Criteria, r.MaxPoints, r.Description, r.DisplayOrder,
        r.IsActive, r.CreatedAt, r.CreatedBy);
}
