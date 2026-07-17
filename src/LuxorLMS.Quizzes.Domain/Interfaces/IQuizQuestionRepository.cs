using LuxorLMS.Quizzes.Domain.Entities;

namespace LuxorLMS.Quizzes.Domain.Interfaces;

public interface IQuizQuestionRepository
{
    Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizQuestion>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task AddAsync(QuizQuestion question, CancellationToken cancellationToken = default);
    Task UpdateAsync(QuizQuestion question, CancellationToken cancellationToken = default);
    Task DeleteAsync(QuizQuestion question, CancellationToken cancellationToken = default);
}
