using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(ICourseRepository repository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<CourseDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByDepartmentIdAsync(departmentId, cancellationToken);
        return Result<IReadOnlyList<CourseDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<CourseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseDto>.Failure(new Error("Course.NotFound", "Course not found."));
        return Result<CourseDto>.Success(Map(entity));
    }

    public async Task<Result<IReadOnlyList<CoursePrerequisiteDto>>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var prerequisites = await _repository.GetPrerequisitesAsync(courseId, cancellationToken);
        return Result<IReadOnlyList<CoursePrerequisiteDto>>.Success(
            prerequisites.Select(p => new CoursePrerequisiteDto(p.PrerequisiteCourseId, string.Empty, string.Empty)).ToList());
    }

    public async Task<Result<CourseDto>> CreateAsync(CreateCourseRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken) is null)
            return Result<CourseDto>.Failure(new Error("Course.InvalidDepartment", "Referenced department does not exist."));

        if (await _repository.ExistsByCodeAsync(request.CourseCode, cancellationToken))
            return Result<CourseDto>.Failure(new Error("Course.DuplicateCode", "A course with this code already exists."));

        var entity = new Course
        {
            Id = Guid.NewGuid(),
            DepartmentId = request.DepartmentId,
            CourseCode = request.CourseCode,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            CreditHours = request.CreditHours,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseDto>.Success(Map(entity));
    }

    public async Task<Result<CourseDto>> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseDto>.Failure(new Error("Course.NotFound", "Course not found."));

        if (await _repository.ExistsByCodeAsync(request.CourseCode, id, cancellationToken))
            return Result<CourseDto>.Failure(new Error("Course.DuplicateCode", "A course with this code already exists."));

        entity.CourseCode = request.CourseCode;
        entity.NameAr = request.NameAr;
        entity.NameEn = request.NameEn;
        entity.CreditHours = request.CreditHours;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Course.NotFound", "Course not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CourseDto Map(Course c) => new(c.Id, c.DepartmentId, c.CourseCode, c.NameAr, c.NameEn, c.CreditHours, c.Description, c.IsActive, c.CreatedAt);
}
