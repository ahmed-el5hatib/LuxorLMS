using LuxorLMS.Quizzes.Domain.Entities;

namespace LuxorLMS.Quizzes.Domain.Interfaces;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetInProgressAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizAttempt>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizAttempt>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizAttempt>> GetByQuizAndStudentAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
    Task DeleteAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
}
