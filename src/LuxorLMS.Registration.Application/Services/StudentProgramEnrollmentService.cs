using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.DTOs;
using LuxorLMS.Registration.Application.Interfaces;
using LuxorLMS.Registration.Domain.Enums;
using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Interfaces;

namespace LuxorLMS.Registration.Application.Services;

public class StudentProgramEnrollmentService : IStudentProgramEnrollmentService
{
    private readonly IStudentProgramEnrollmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentProgramEnrollmentService(IStudentProgramEnrollmentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<StudentProgramEnrollmentDto>>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<StudentProgramEnrollmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<StudentProgramEnrollmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentProgramEnrollmentDto>.Failure(new Error("StudentProgramEnrollment.NotFound", "Student program enrollment not found."));
        return Result<StudentProgramEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result<StudentProgramEnrollmentDto>> CreateAsync(CreateStudentProgramEnrollmentRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetActiveByStudentAndProgramAsync(request.StudentId, request.ProgramId, cancellationToken) is not null)
            return Result<StudentProgramEnrollmentDto>.Failure(new Error("StudentProgramEnrollment.Duplicate", "Student is already actively enrolled in this program."));

        var entity = new StudentProgramEnrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            ProgramId = request.ProgramId,
            EnrollmentDate = DateTime.UtcNow,
            Status = StudentProgramStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentProgramEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result<StudentProgramEnrollmentDto>> UpdateAsync(Guid id, UpdateStudentProgramEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<StudentProgramEnrollmentDto>.Failure(new Error("StudentProgramEnrollment.NotFound", "Student program enrollment not found."));

        entity.Status = request.Status;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<StudentProgramEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("StudentProgramEnrollment.NotFound", "Student program enrollment not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static StudentProgramEnrollmentDto Map(StudentProgramEnrollment e) => new(e.Id, e.StudentId, e.ProgramId, e.EnrollmentDate, e.Status, e.CreatedAt);
}
