using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly IFacultyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FacultyService(IFacultyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<FacultyDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<FacultyDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<FacultyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<FacultyDto>.Failure(new Error("Faculty.NotFound", "Faculty not found."));
        return Result<FacultyDto>.Success(Map(entity));
    }

    public async Task<Result<FacultyDto>> CreateAsync(CreateFacultyRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByCodeAsync(request.Code, cancellationToken))
            return Result<FacultyDto>.Failure(new Error("Faculty.DuplicateCode", "A faculty with this code already exists."));

        var entity = new Faculty
        {
            Id = Guid.NewGuid(),
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            Code = request.Code,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<FacultyDto>.Success(Map(entity));
    }

    public async Task<Result<FacultyDto>> UpdateAsync(Guid id, UpdateFacultyRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<FacultyDto>.Failure(new Error("Faculty.NotFound", "Faculty not found."));

        if (await _repository.ExistsByCodeAsync(request.Code, id, cancellationToken))
            return Result<FacultyDto>.Failure(new Error("Faculty.DuplicateCode", "A faculty with this code already exists."));

        entity.NameAr = request.NameAr;
        entity.NameEn = request.NameEn;
        entity.Code = request.Code;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<FacultyDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Faculty.NotFound", "Faculty not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static FacultyDto Map(Faculty f) => new(f.Id, f.NameAr, f.NameEn, f.Code, f.IsActive, f.CreatedAt);
}
