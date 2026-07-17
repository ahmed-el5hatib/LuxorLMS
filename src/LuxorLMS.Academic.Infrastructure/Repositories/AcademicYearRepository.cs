using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class AcademicYearRepository : IAcademicYearRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public AcademicYearRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicYear?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AcademicYears.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<AcademicYear>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.AcademicYears.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsByLabelAsync(string label, CancellationToken cancellationToken = default)
        => await _context.AcademicYears.AnyAsync(ay => ay.Label == label, cancellationToken);

    public async Task<bool> ExistsByLabelAsync(string label, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.AcademicYears.AnyAsync(ay => ay.Label == label && ay.Id != excludingId, cancellationToken);

    public async Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
        => await _context.AcademicYears.AddAsync(academicYear, cancellationToken);

    public Task UpdateAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        _context.AcademicYears.Update(academicYear);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        _context.AcademicYears.Remove(academicYear);
        return Task.CompletedTask;
    }
}
