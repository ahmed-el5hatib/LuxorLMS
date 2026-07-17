using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public SemesterRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Semester?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Semesters.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Semester>> GetByAcademicYearIdAsync(Guid academicYearId, CancellationToken cancellationToken = default)
        => await _context.Semesters.AsNoTracking()
            .Where(s => s.AcademicYearId == academicYearId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Semester semester, CancellationToken cancellationToken = default)
        => await _context.Semesters.AddAsync(semester, cancellationToken);

    public Task UpdateAsync(Semester semester, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Update(semester);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Semester semester, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Remove(semester);
        return Task.CompletedTask;
    }
}
