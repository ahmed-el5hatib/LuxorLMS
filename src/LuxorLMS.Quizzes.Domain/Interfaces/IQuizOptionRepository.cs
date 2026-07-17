using LuxorLMS.Quizzes.Domain.Entities;

namespace LuxorLMS.Quizzes.Domain.Interfaces;

public interface IQuizOptionRepository
{
    Task<QuizOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizOption>> GetByQuestionIdAsync(Guid quizQuestionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizOption>> GetByQuestionIdsAsync(IReadOnlyCollection<Guid> quizQuestionIds, CancellationToken cancellationToken = default);
    Task AddAsync(QuizOption option, CancellationToken cancellationToken = default);
    Task UpdateAsync(QuizOption option, CancellationToken cancellationToken = default);
    Task DeleteAsync(QuizOption option, CancellationToken cancellationToken = default);
}
