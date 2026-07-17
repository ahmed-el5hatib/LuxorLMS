using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class ProgramRepository : IProgramRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public ProgramRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Program?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Programs.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Program>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
        => await _context.Programs.AsNoTracking()
            .Where(p => p.DepartmentId == departmentId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Programs.AnyAsync(p => p.Code == code, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.Programs.AnyAsync(p => p.Code == code && p.Id != excludingId, cancellationToken);

    public async Task AddAsync(Program program, CancellationToken cancellationToken = default)
        => await _context.Programs.AddAsync(program, cancellationToken);

    public Task UpdateAsync(Program program, CancellationToken cancellationToken = default)
    {
        _context.Programs.Update(program);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Program program, CancellationToken cancellationToken = default)
    {
        _context.Programs.Remove(program);
        return Task.CompletedTask;
    }
}
