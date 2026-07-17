using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Assignments.Infrastructure.Repositories;

public class AssignmentRubricRepository : IAssignmentRubricRepository
{
    private readonly LuxorLMSAssignmentsDbContext _context;

    public AssignmentRubricRepository(LuxorLMSAssignmentsDbContext context)
    {
        _context = context;
    }

    public async Task<AssignmentRubric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AssignmentRubrics.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<AssignmentRubric>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await _context.AssignmentRubrics.AsNoTracking()
            .Where(r => r.AssignmentId == assignmentId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default)
        => await _context.AssignmentRubrics.AddAsync(rubric, cancellationToken);

    public Task UpdateAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default)
    {
        _context.AssignmentRubrics.Update(rubric);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AssignmentRubric rubric, CancellationToken cancellationToken = default)
    {
        _context.AssignmentRubrics.Remove(rubric);
        return Task.CompletedTask;
    }
}
