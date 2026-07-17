using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Quizzes.Infrastructure.Repositories;

public class QuizQuestionRepository : IQuizQuestionRepository
{
    private readonly LuxorLMSQuizzesDbContext _context;

    public QuizQuestionRepository(LuxorLMSQuizzesDbContext context)
    {
        _context = context;
    }

    public async Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.QuizQuestions.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<QuizQuestion>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
        => await _context.QuizQuestions.AsNoTracking()
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(QuizQuestion question, CancellationToken cancellationToken = default)
        => await _context.QuizQuestions.AddAsync(question, cancellationToken);

    public Task UpdateAsync(QuizQuestion question, CancellationToken cancellationToken = default)
    {
        _context.QuizQuestions.Update(question);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(QuizQuestion question, CancellationToken cancellationToken = default)
    {
        _context.QuizQuestions.Remove(question);
        return Task.CompletedTask;
    }
}
