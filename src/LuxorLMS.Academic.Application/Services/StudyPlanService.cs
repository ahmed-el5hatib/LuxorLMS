using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class StudyPlanService : IStudyPlanService
{
    private readonly IStudyPlanRepository _repository;
    private readonly IProgramRepository _programRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudyPlanService(IStudyPlanRepository repository, IProgramRepository programRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _programRepository = programRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<StudyPlanDto>>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByProgramIdAsync(programId, cancellationToken);
        return Result<IReadOnlyList<StudyPlanDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<StudyPlanDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudyPlanDto>.Failure(new Error("StudyPlan.NotFound", "Study plan not found."));
        return Result<StudyPlanDto>.Success(Map(entity));
    }

    public async Task<Result<StudyPlanDto>> CreateAsync(CreateStudyPlanRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _programRepository.GetByIdAsync(request.ProgramId, cancellationToken) is null)
            return Result<StudyPlanDto>.Failure(new Error("StudyPlan.InvalidProgram", "Referenced program does not exist."));

        if (await _repository.ExistsByVersionCodeAsync(request.VersionCode, cancellationToken))
            return Result<StudyPlanDto>.Failure(new Error("StudyPlan.DuplicateVersion", "A study plan with this version code already exists."));

        var entity = new StudyPlan
        {
            Id = Guid.NewGuid(),
            ProgramId = request.ProgramId,
            VersionCode = request.VersionCode,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            MinimumCredits = request.MinimumCredits,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudyPlanDto>.Success(Map(entity));
    }

    public async Task<Result<StudyPlanDto>> UpdateAsync(Guid id, UpdateStudyPlanRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudyPlanDto>.Failure(new Error("StudyPlan.NotFound", "Study plan not found."));

        if (await _repository.ExistsByVersionCodeAsync(request.VersionCode, id, cancellationToken))
            return Result<StudyPlanDto>.Failure(new Error("StudyPlan.DuplicateVersion", "A study plan with this version code already exists."));

        entity.VersionCode = request.VersionCode;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.MinimumCredits = request.MinimumCredits;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudyPlanDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("StudyPlan.NotFound", "Study plan not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static StudyPlanDto Map(StudyPlan sp) => new(sp.Id, sp.ProgramId, sp.VersionCode, sp.EffectiveFrom, sp.EffectiveTo, sp.MinimumCredits, sp.IsActive, sp.CreatedAt);
}
