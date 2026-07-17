using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Enums;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class CourseOfferingService : ICourseOfferingService
{
    private readonly ICourseOfferingRepository _repository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourseOfferingService(ICourseOfferingRepository repository, ICourseRepository courseRepository, ISemesterRepository semesterRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _courseRepository = courseRepository;
        _semesterRepository = semesterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<CourseOfferingDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetBySemesterIdAsync(semesterId, cancellationToken);
        return Result<IReadOnlyList<CourseOfferingDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<CourseOfferingDto>>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByCourseIdAsync(courseId, cancellationToken);
        return Result<IReadOnlyList<CourseOfferingDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<CourseOfferingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseOfferingDto>.Failure(new Error("CourseOffering.NotFound", "Course offering not found."));
        return Result<CourseOfferingDto>.Success(Map(entity));
    }

    public async Task<Result<CourseOfferingDto>> CreateAsync(CreateCourseOfferingRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken) is null)
            return Result<CourseOfferingDto>.Failure(new Error("CourseOffering.InvalidCourse", "Referenced course does not exist."));
        if (await _semesterRepository.GetByIdAsync(request.SemesterId, cancellationToken) is null)
            return Result<CourseOfferingDto>.Failure(new Error("CourseOffering.InvalidSemester", "Referenced semester does not exist."));

        var entity = new CourseOffering
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            SemesterId = request.SemesterId,
            PrimaryTeacherId = request.PrimaryTeacherId,
            Capacity = request.Capacity,
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            Status = OfferingStatus.Draft,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseOfferingDto>.Success(Map(entity));
    }

    public async Task<Result<CourseOfferingDto>> UpdateAsync(Guid id, UpdateCourseOfferingRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseOfferingDto>.Failure(new Error("CourseOffering.NotFound", "Course offering not found."));

        entity.Capacity = request.Capacity;
        entity.RegistrationStart = request.RegistrationStart;
        entity.RegistrationEnd = request.RegistrationEnd;
        entity.Status = request.Status;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseOfferingDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("CourseOffering.NotFound", "Course offering not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CourseOfferingDto Map(CourseOffering o) => new(o.Id, o.CourseId, o.SemesterId, o.PrimaryTeacherId, o.Capacity, o.RegistrationStart, o.RegistrationEnd, o.Status, o.IsActive, o.CreatedAt);
}
