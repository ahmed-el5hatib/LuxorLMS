using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(ISemesterRepository repository, IAcademicYearRepository academicYearRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<SemesterDto>>> GetByAcademicYearIdAsync(Guid academicYearId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByAcademicYearIdAsync(academicYearId, cancellationToken);
        return Result<IReadOnlyList<SemesterDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<SemesterDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<SemesterDto>.Failure(new Error("Semester.NotFound", "Semester not found."));
        return Result<SemesterDto>.Success(Map(entity));
    }

    public async Task<Result<SemesterDto>> CreateAsync(CreateSemesterRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _academicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken) is null)
            return Result<SemesterDto>.Failure(new Error("Semester.InvalidAcademicYear", "Referenced academic year does not exist."));

        var entity = new Semester
        {
            Id = Guid.NewGuid(),
            AcademicYearId = request.AcademicYearId,
            Name = request.Name,
            Code = request.Code,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SemesterDto>.Success(Map(entity));
    }

    public async Task<Result<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<SemesterDto>.Failure(new Error("Semester.NotFound", "Semester not found."));

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.RegistrationStart = request.RegistrationStart;
        entity.RegistrationEnd = request.RegistrationEnd;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SemesterDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Semester.NotFound", "Semester not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static SemesterDto Map(Semester s) => new(s.Id, s.AcademicYearId, s.Name, s.Code, s.StartDate, s.EndDate, s.RegistrationStart, s.RegistrationEnd, s.IsActive, s.CreatedAt);
}
