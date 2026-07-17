using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Quizzes.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly LuxorLMSQuizzesDbContext _context;

    public QuizRepository(LuxorLMSQuizzesDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Quizzes.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Quiz>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.Quizzes.AsNoTracking()
            .Where(q => q.CourseOfferingId == courseOfferingId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Quiz>> GetPublishedByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.Quizzes.AsNoTracking()
            .Where(q => q.CourseOfferingId == courseOfferingId && q.IsPublished && q.IsActive)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default)
        => await _context.Quizzes.AddAsync(quiz, cancellationToken);

    public Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        _context.Quizzes.Update(quiz);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        _context.Quizzes.Remove(quiz);
        return Task.CompletedTask;
    }
}
