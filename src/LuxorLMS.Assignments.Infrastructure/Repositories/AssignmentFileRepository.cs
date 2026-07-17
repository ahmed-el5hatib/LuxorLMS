using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Assignments.Infrastructure.Repositories;

public class AssignmentFileRepository : IAssignmentFileRepository
{
    private readonly LuxorLMSAssignmentsDbContext _context;

    public AssignmentFileRepository(LuxorLMSAssignmentsDbContext context)
    {
        _context = context;
    }

    public async Task<AssignmentFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AssignmentFiles.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<AssignmentFile>> GetBySubmissionIdAsync(Guid assignmentSubmissionId, CancellationToken cancellationToken = default)
        => await _context.AssignmentFiles.AsNoTracking()
            .Where(f => f.AssignmentSubmissionId == assignmentSubmissionId)
            .OrderBy(f => f.Version)
            .ToListAsync(cancellationToken);

    public async Task<int> GetNextVersionAsync(Guid assignmentSubmissionId, CancellationToken cancellationToken = default)
    {
        var max = await _context.AssignmentFiles.AsNoTracking()
            .Where(f => f.AssignmentSubmissionId == assignmentSubmissionId && f.IsActive)
            .MaxAsync(f => (int?)f.Version, cancellationToken);
        return (max ?? 0) + 1;
    }

    public async Task AddAsync(AssignmentFile file, CancellationToken cancellationToken = default)
        => await _context.AssignmentFiles.AddAsync(file, cancellationToken);

    public Task UpdateAsync(AssignmentFile file, CancellationToken cancellationToken = default)
    {
        _context.AssignmentFiles.Update(file);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AssignmentFile file, CancellationToken cancellationToken = default)
    {
        _context.AssignmentFiles.Remove(file);
        return Task.CompletedTask;
    }
}
