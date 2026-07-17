using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Assignments.Infrastructure.Repositories;

public class AssignmentSubmissionRepository : IAssignmentSubmissionRepository
{
    private readonly LuxorLMSAssignmentsDbContext _context;

    public AssignmentSubmissionRepository(LuxorLMSAssignmentsDbContext context)
    {
        _context = context;
    }

    public async Task<AssignmentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AssignmentSubmissions.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        => await _context.AssignmentSubmissions.AsNoTracking()
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AssignmentSubmission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.AssignmentSubmissions.AsNoTracking()
            .Where(s => s.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task<AssignmentSubmission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.AssignmentSubmissions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, cancellationToken);

    public async Task AddAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default)
        => await _context.AssignmentSubmissions.AddAsync(submission, cancellationToken);

    public Task UpdateAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default)
    {
        _context.AssignmentSubmissions.Update(submission);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default)
    {
        _context.AssignmentSubmissions.Remove(submission);
        return Task.CompletedTask;
    }
}
