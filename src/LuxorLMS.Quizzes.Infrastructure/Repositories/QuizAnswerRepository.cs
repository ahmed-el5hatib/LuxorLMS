using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Quizzes.Infrastructure.Repositories;

public class QuizAnswerRepository : IQuizAnswerRepository
{
    private readonly LuxorLMSQuizzesDbContext _context;

    public QuizAnswerRepository(LuxorLMSQuizzesDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAnswer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.QuizAnswers.FindAsync(new object[] { id }, cancellationToken);

    public async Task<QuizAnswer?> GetByAttemptAndQuestionAsync(Guid quizAttemptId, Guid quizQuestionId, CancellationToken cancellationToken = default)
        => await _context.QuizAnswers
            .FirstOrDefaultAsync(a => a.QuizAttemptId == quizAttemptId && a.QuizQuestionId == quizQuestionId, cancellationToken);

    public async Task<IReadOnlyList<QuizAnswer>> GetByAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
        => await _context.QuizAnswers.AsNoTracking()
            .Where(a => a.QuizAttemptId == quizAttemptId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(QuizAnswer answer, CancellationToken cancellationToken = default)
        => await _context.QuizAnswers.AddAsync(answer, cancellationToken);

    public Task UpdateAsync(QuizAnswer answer, CancellationToken cancellationToken = default)
    {
        _context.QuizAnswers.Update(answer);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(QuizAnswer answer, CancellationToken cancellationToken = default)
    {
        _context.QuizAnswers.Remove(answer);
        return Task.CompletedTask;
    }
}
