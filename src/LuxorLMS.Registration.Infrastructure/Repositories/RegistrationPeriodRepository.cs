using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Interfaces;
using LuxorLMS.Registration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Registration.Infrastructure.Repositories;

public class RegistrationPeriodRepository : IRegistrationPeriodRepository
{
    private readonly LuxorLMSRegistrationDbContext _context;

    public RegistrationPeriodRepository(LuxorLMSRegistrationDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.RegistrationPeriods.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<RegistrationPeriod>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.RegistrationPeriods.AsNoTracking()
            .Where(rp => rp.SemesterId == semesterId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RegistrationPeriod>> GetActiveAsync(DateTime now, CancellationToken cancellationToken = default)
        => await _context.RegistrationPeriods.AsNoTracking()
            .Where(rp => rp.IsActive && rp.StartDate <= now && rp.EndDate >= now)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(RegistrationPeriod period, CancellationToken cancellationToken = default)
        => await _context.RegistrationPeriods.AddAsync(period, cancellationToken);

    public Task UpdateAsync(RegistrationPeriod period, CancellationToken cancellationToken = default)
    {
        _context.RegistrationPeriods.Update(period);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RegistrationPeriod period, CancellationToken cancellationToken = default)
    {
        _context.RegistrationPeriods.Remove(period);
        return Task.CompletedTask;
    }
}
