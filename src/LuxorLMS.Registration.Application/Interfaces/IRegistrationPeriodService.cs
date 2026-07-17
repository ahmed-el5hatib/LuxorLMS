using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.DTOs;

namespace LuxorLMS.Registration.Application.Interfaces;

public interface IRegistrationPeriodService
{
    Task<Result<IReadOnlyList<RegistrationPeriodDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<RegistrationPeriodDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<RegistrationPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<RegistrationPeriodDto>> CreateAsync(CreateRegistrationPeriodRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<RegistrationPeriodDto>> UpdateAsync(Guid id, UpdateRegistrationPeriodRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
