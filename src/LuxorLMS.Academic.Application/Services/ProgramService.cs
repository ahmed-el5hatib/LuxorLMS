using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IProgramRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProgramService(IProgramRepository repository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<ProgramDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByDepartmentIdAsync(departmentId, cancellationToken);
        return Result<IReadOnlyList<ProgramDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<ProgramDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<ProgramDto>.Failure(new Error("Program.NotFound", "Program not found."));
        return Result<ProgramDto>.Success(Map(entity));
    }

    public async Task<Result<ProgramDto>> CreateAsync(CreateProgramRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken) is null)
            return Result<ProgramDto>.Failure(new Error("Program.InvalidDepartment", "Referenced department does not exist."));

        if (await _repository.ExistsByCodeAsync(request.Code, cancellationToken))
            return Result<ProgramDto>.Failure(new Error("Program.DuplicateCode", "A program with this code already exists."));

        var entity = new Program
        {
            Id = Guid.NewGuid(),
            DepartmentId = request.DepartmentId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            Code = request.Code,
            DegreeLevel = request.DegreeLevel,
            TotalCreditsRequired = request.TotalCreditsRequired,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ProgramDto>.Success(Map(entity));
    }

    public async Task<Result<ProgramDto>> UpdateAsync(Guid id, UpdateProgramRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<ProgramDto>.Failure(new Error("Program.NotFound", "Program not found."));

        if (await _repository.ExistsByCodeAsync(request.Code, id, cancellationToken))
            return Result<ProgramDto>.Failure(new Error("Program.DuplicateCode", "A program with this code already exists."));

        entity.NameAr = request.NameAr;
        entity.NameEn = request.NameEn;
        entity.Code = request.Code;
        entity.DegreeLevel = request.DegreeLevel;
        entity.TotalCreditsRequired = request.TotalCreditsRequired;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ProgramDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Program.NotFound", "Program not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static ProgramDto Map(Program p) => new(p.Id, p.DepartmentId, p.NameAr, p.NameEn, p.Code, p.DegreeLevel, p.TotalCreditsRequired, p.IsActive, p.CreatedAt);
}
