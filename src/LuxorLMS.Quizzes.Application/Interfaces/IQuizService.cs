using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Interfaces;

public interface IQuizService
{
    // Quizzes
    Task<Result<IReadOnlyList<QuizDto>>> GetByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<QuizDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<QuizDetailDto>> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<QuizDto>> CreateAsync(CreateQuizRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<QuizDto>> UpdateAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken = default);
    Task<Result<QuizDto>> PublishAsync(Guid id, bool publish, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Visual builder (creates quiz + questions + options in one transaction)
    Task<Result<QuizDetailDto>> BuildAsync(QuizBuilderRequest request, Guid createdBy, CancellationToken cancellationToken = default);

    // Questions
    Task<Result<IReadOnlyList<QuizQuestionDto>>> GetQuestionsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<Result<QuizQuestionDto>> GetQuestionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<QuizQuestionDto>> CreateQuestionAsync(CreateQuizQuestionRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<QuizQuestionDto>> UpdateQuestionAsync(Guid id, UpdateQuizQuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteQuestionAsync(Guid id, CancellationToken cancellationToken = default);

    // Options
    Task<Result<IReadOnlyList<QuizOptionDto>>> GetOptionsByQuestionAsync(Guid quizQuestionId, CancellationToken cancellationToken = default);
    Task<Result<QuizOptionDto>> GetOptionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<QuizOptionDto>> CreateOptionAsync(CreateQuizOptionRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<QuizOptionDto>> UpdateOptionAsync(Guid id, UpdateQuizOptionRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteOptionAsync(Guid id, CancellationToken cancellationToken = default);
}
