using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IDepartmentRepository repository, IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>>> GetByFacultyIdAsync(Guid facultyId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByFacultyIdAsync(facultyId, cancellationToken);
        return Result<IReadOnlyList<DepartmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<DepartmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<DepartmentDto>.Failure(new Error("Department.NotFound", "Department not found."));
        return Result<DepartmentDto>.Success(Map(entity));
    }

    public async Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken) is null)
            return Result<DepartmentDto>.Failure(new Error("Department.InvalidFaculty", "Referenced faculty does not exist."));

        if (await _repository.ExistsByCodeAsync(request.Code, cancellationToken))
            return Result<DepartmentDto>.Failure(new Error("Department.DuplicateCode", "A department with this code already exists."));

        var entity = new Department
        {
            Id = Guid.NewGuid(),
            FacultyId = request.FacultyId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            Code = request.Code,
            HeadId = request.HeadId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<DepartmentDto>.Success(Map(entity));
    }

    public async Task<Result<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<DepartmentDto>.Failure(new Error("Department.NotFound", "Department not found."));

        if (await _repository.ExistsByCodeAsync(request.Code, id, cancellationToken))
            return Result<DepartmentDto>.Failure(new Error("Department.DuplicateCode", "A department with this code already exists."));

        entity.NameAr = request.NameAr;
        entity.NameEn = request.NameEn;
        entity.Code = request.Code;
        entity.HeadId = request.HeadId;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<DepartmentDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Department.NotFound", "Department not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static DepartmentDto Map(Department d) => new(d.Id, d.FacultyId, d.NameAr, d.NameEn, d.Code, d.HeadId, d.IsActive, d.CreatedAt);
}
