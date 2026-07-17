using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Quizzes.Infrastructure.Repositories;

public class QuizOptionRepository : IQuizOptionRepository
{
    private readonly LuxorLMSQuizzesDbContext _context;

    public QuizOptionRepository(LuxorLMSQuizzesDbContext context)
    {
        _context = context;
    }

    public async Task<QuizOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.QuizOptions.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<QuizOption>> GetByQuestionIdAsync(Guid quizQuestionId, CancellationToken cancellationToken = default)
        => await _context.QuizOptions.AsNoTracking()
            .Where(o => o.QuizQuestionId == quizQuestionId)
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<QuizOption>> GetByQuestionIdsAsync(IReadOnlyCollection<Guid> quizQuestionIds, CancellationToken cancellationToken = default)
    {
        if (quizQuestionIds is null || quizQuestionIds.Count == 0)
            return new List<QuizOption>();

        return await _context.QuizOptions.AsNoTracking()
            .Where(o => quizQuestionIds.Contains(o.QuizQuestionId))
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(QuizOption option, CancellationToken cancellationToken = default)
        => await _context.QuizOptions.AddAsync(option, cancellationToken);

    public Task UpdateAsync(QuizOption option, CancellationToken cancellationToken = default)
    {
        _context.QuizOptions.Update(option);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(QuizOption option, CancellationToken cancellationToken = default)
    {
        _context.QuizOptions.Remove(option);
        return Task.CompletedTask;
    }
}
