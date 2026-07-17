using LuxorLMS.Quizzes.Domain.Entities;

namespace LuxorLMS.Quizzes.Domain.Interfaces;

public interface IQuizAnswerRepository
{
    Task<QuizAnswer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuizAnswer?> GetByAttemptAndQuestionAsync(Guid quizAttemptId, Guid quizQuestionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizAnswer>> GetByAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task AddAsync(QuizAnswer answer, CancellationToken cancellationToken = default);
    Task UpdateAsync(QuizAnswer answer, CancellationToken cancellationToken = default);
    Task DeleteAsync(QuizAnswer answer, CancellationToken cancellationToken = default);
}
