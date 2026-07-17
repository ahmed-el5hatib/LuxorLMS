using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Interfaces;

public interface IQuizAttemptService
{
    Task<Result<IReadOnlyList<QuizAttemptDto>>> GetByQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<QuizAttemptDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<QuizAttemptDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<QuizAttemptDto>> StartAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<QuizAttemptDto>> SubmitAsync(Guid attemptId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Force-expires an in-progress attempt whose time limit has elapsed (auto-submit).</summary>
    Task<Result<QuizAttemptDto>> AutoSubmitIfExpiredAsync(Guid attemptId, CancellationToken cancellationToken = default);
}
