using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public DepartmentRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Departments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Department>> GetByFacultyIdAsync(Guid facultyId, CancellationToken cancellationToken = default)
        => await _context.Departments.AsNoTracking()
            .Where(d => d.FacultyId == facultyId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Departments.AnyAsync(d => d.Code == code, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.Departments.AnyAsync(d => d.Code == code && d.Id != excludingId, cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
        => await _context.Departments.AddAsync(department, cancellationToken);

    public Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Remove(department);
        return Task.CompletedTask;
    }
}
