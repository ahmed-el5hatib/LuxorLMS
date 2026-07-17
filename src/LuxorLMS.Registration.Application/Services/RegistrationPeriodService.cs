using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.DTOs;
using LuxorLMS.Registration.Application.Interfaces;
using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Interfaces;

namespace LuxorLMS.Registration.Application.Services;

public class RegistrationPeriodService : IRegistrationPeriodService
{
    private readonly IRegistrationPeriodRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationPeriodService(IRegistrationPeriodRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<RegistrationPeriodDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetBySemesterIdAsync(semesterId, cancellationToken);
        return Result<IReadOnlyList<RegistrationPeriodDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<RegistrationPeriodDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetActiveAsync(DateTime.UtcNow, cancellationToken);
        return Result<IReadOnlyList<RegistrationPeriodDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<RegistrationPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<RegistrationPeriodDto>.Failure(new Error("RegistrationPeriod.NotFound", "Registration period not found."));
        return Result<RegistrationPeriodDto>.Success(Map(entity));
    }

    public async Task<Result<RegistrationPeriodDto>> CreateAsync(CreateRegistrationPeriodRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RegistrationPeriod
        {
            Id = Guid.NewGuid(),
            SemesterId = request.SemesterId,
            ProgramId = request.ProgramId,
            AcademicYearId = request.AcademicYearId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            LateRegistrationStart = request.LateRegistrationStart,
            LateRegistrationEnd = request.LateRegistrationEnd,
            MinCreditHours = request.MinCreditHours,
            MaxCreditHours = request.MaxCreditHours,
            GpaCapForMax = request.GpaCapForMax,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<RegistrationPeriodDto>.Success(Map(entity));
    }

    public async Task<Result<RegistrationPeriodDto>> UpdateAsync(Guid id, UpdateRegistrationPeriodRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<RegistrationPeriodDto>.Failure(new Error("RegistrationPeriod.NotFound", "Registration period not found."));

        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.LateRegistrationStart = request.LateRegistrationStart;
        entity.LateRegistrationEnd = request.LateRegistrationEnd;
        entity.MinCreditHours = request.MinCreditHours;
        entity.MaxCreditHours = request.MaxCreditHours;
        entity.GpaCapForMax = request.GpaCapForMax;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<RegistrationPeriodDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("RegistrationPeriod.NotFound", "Registration period not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static RegistrationPeriodDto Map(RegistrationPeriod rp) => new(
        rp.Id, rp.SemesterId, rp.ProgramId, rp.AcademicYearId, rp.StartDate, rp.EndDate,
        rp.LateRegistrationStart, rp.LateRegistrationEnd, rp.MinCreditHours, rp.MaxCreditHours, rp.GpaCapForMax, rp.IsActive, rp.CreatedAt);
}
