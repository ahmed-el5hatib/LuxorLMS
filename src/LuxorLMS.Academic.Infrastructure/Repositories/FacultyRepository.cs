using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class FacultyRepository : IFacultyRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public FacultyRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Faculties.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Faculty>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Faculties.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Faculties.AnyAsync(f => f.Code == code, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.Faculties.AnyAsync(f => f.Code == code && f.Id != excludingId, cancellationToken);

    public async Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default)
        => await _context.Faculties.AddAsync(faculty, cancellationToken);

    public Task UpdateAsync(Faculty faculty, CancellationToken cancellationToken = default)
    {
        _context.Faculties.Update(faculty);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Faculty faculty, CancellationToken cancellationToken = default)
    {
        _context.Faculties.Remove(faculty);
        return Task.CompletedTask;
    }
}
