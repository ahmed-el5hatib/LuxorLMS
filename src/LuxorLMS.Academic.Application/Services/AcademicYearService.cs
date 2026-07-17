using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class AcademicYearService : IAcademicYearService
{
    private readonly IAcademicYearRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AcademicYearService(IAcademicYearRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AcademicYearDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<AcademicYearDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<AcademicYearDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AcademicYearDto>.Failure(new Error("AcademicYear.NotFound", "Academic year not found."));
        return Result<AcademicYearDto>.Success(Map(entity));
    }

    public async Task<Result<AcademicYearDto>> CreateAsync(CreateAcademicYearRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByLabelAsync(request.Label, cancellationToken))
            return Result<AcademicYearDto>.Failure(new Error("AcademicYear.DuplicateLabel", "An academic year with this label already exists."));

        var entity = new AcademicYear
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AcademicYearDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("AcademicYear.NotFound", "Academic year not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static AcademicYearDto Map(AcademicYear ay) => new(ay.Id, ay.Label, ay.StartDate, ay.EndDate, ay.IsActive, ay.CreatedAt);
}
