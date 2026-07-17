using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Interfaces;

public interface IQuizAnswerService
{
    Task<Result<IReadOnlyList<QuizAnswerDto>>> GetByAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<Result<QuizAnswerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Saves (creates or updates) an answer for a question in an in-progress attempt, auto-grading objective questions.</summary>
    Task<Result<QuizAnswerDto>> SaveAsync(SaveAnswerRequest request, Guid createdBy, CancellationToken cancellationToken = default);

    /// <summary>Manually grade an essay answer (teacher).</summary>
    Task<Result<QuizAnswerDto>> GradeAsync(Guid id, bool isCorrect, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
