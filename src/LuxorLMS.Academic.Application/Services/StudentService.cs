using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<decimal>> GetGpaByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var student = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (student is null)
            return Result<decimal>.Success(4.0m);

        return Result<decimal>.Success(student.Gpa);
    }
}
