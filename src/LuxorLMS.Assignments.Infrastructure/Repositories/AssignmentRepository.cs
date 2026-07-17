using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Assignments.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly LuxorLMSAssignmentsDbContext _context;

    public AssignmentRepository(LuxorLMSAssignmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Assignments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Assignment>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.Assignments.AsNoTracking()
            .Where(a => a.CourseOfferingId == courseOfferingId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Assignment>> GetActiveByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.Assignments.AsNoTracking()
            .Where(a => a.CourseOfferingId == courseOfferingId && a.IsActive)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
        => await _context.Assignments.AddAsync(assignment, cancellationToken);

    public Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Update(assignment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Remove(assignment);
        return Task.CompletedTask;
    }
}
