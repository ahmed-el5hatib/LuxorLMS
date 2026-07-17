using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface IStudentService
{
    Task<Result<decimal>> GetGpaByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
