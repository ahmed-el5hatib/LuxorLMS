using LuxorLMS.Registration.Domain.Entities;

namespace LuxorLMS.Registration.Domain.Interfaces;

public interface IRegistrationPeriodRepository
{
    Task<RegistrationPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationPeriod>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistrationPeriod>> GetActiveAsync(DateTime now, CancellationToken cancellationToken = default);
    Task AddAsync(RegistrationPeriod period, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegistrationPeriod period, CancellationToken cancellationToken = default);
    Task DeleteAsync(RegistrationPeriod period, CancellationToken cancellationToken = default);
}
