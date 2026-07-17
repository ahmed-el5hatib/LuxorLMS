using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Enums;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Quizzes.Infrastructure.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly LuxorLMSQuizzesDbContext _context;

    public QuizAttemptRepository(LuxorLMSQuizzesDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts.FindAsync(new object[] { id }, cancellationToken);

    public async Task<QuizAttempt?> GetInProgressAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts
            .FirstOrDefaultAsync(a => a.QuizId == quizId && a.StudentId == studentId && a.Status == QuizAttemptStatus.InProgress, cancellationToken);

    public async Task<IReadOnlyList<QuizAttempt>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts.AsNoTracking()
            .Where(a => a.QuizId == quizId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<QuizAttempt>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts.AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<QuizAttempt>> GetByQuizAndStudentAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts.AsNoTracking()
            .Where(a => a.QuizId == quizId && a.StudentId == studentId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
        => await _context.QuizAttempts.AddAsync(attempt, cancellationToken);

    public Task UpdateAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        _context.QuizAttempts.Update(attempt);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        _context.QuizAttempts.Remove(attempt);
        return Task.CompletedTask;
    }
}
