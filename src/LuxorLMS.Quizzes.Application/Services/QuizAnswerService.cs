using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Enums;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Services;

public class QuizAnswerService : IQuizAnswerService
{
    private readonly IQuizAnswerRepository _answerRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly IQuizOptionRepository _optionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuizAnswerService(
        IQuizAnswerRepository answerRepository,
        IQuizAttemptRepository attemptRepository,
        IQuizQuestionRepository questionRepository,
        IQuizOptionRepository optionRepository,
        IUnitOfWork unitOfWork)
    {
        _answerRepository = answerRepository;
        _attemptRepository = attemptRepository;
        _questionRepository = questionRepository;
        _optionRepository = optionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<QuizAnswerDto>>> GetByAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        var items = await _answerRepository.GetByAttemptIdAsync(quizAttemptId, cancellationToken);
        return Result<IReadOnlyList<QuizAnswerDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<QuizAnswerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _answerRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.NotFound", "Quiz answer not found."));
        return Result<QuizAnswerDto>.Success(Map(entity));
    }

    public async Task<Result<QuizAnswerDto>> SaveAsync(SaveAnswerRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var attempt = await _attemptRepository.GetByIdAsync(request.QuizAttemptId, cancellationToken);
        if (attempt is null) return Result<QuizAnswerDto>.Failure(new Error("QuizAttempt.NotFound", "Quiz attempt not found."));

        if (attempt.Status != QuizAttemptStatus.InProgress)
            return Result<QuizAnswerDto>.Failure(new Error("QuizAttempt.NotInProgress", "Answers can only be saved while the attempt is in progress."));

        var question = await _questionRepository.GetByIdAsync(request.QuizQuestionId, cancellationToken);
        if (question is null) return Result<QuizAnswerDto>.Failure(new Error("QuizQuestion.NotFound", "Quiz question not found."));

        if (question.QuizId != attempt.QuizId)
            return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.Mismatch", "Question does not belong to the quiz being attempted."));

        // Validate payload shape by question type + auto-grade objective questions.
        bool isCorrect = false;
        if (question.QuestionType == QuestionType.Essay)
        {
            if (string.IsNullOrWhiteSpace(request.TextAnswer))
                return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.MissingText", "Essay questions require a text answer."));
            // Essays are graded manually; default to not correct until a teacher grades it.
            isCorrect = false;
        }
        else
        {
            if (request.SelectedOptionId is null)
                return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.MissingOption", "This question requires a selected option."));

            var option = await _optionRepository.GetByIdAsync(request.SelectedOptionId.Value, cancellationToken);
            if (option is null || option.QuizQuestionId != question.Id)
                return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.InvalidOption", "Selected option does not belong to the question."));

            isCorrect = option.IsCorrect;
        }

        var existing = await _answerRepository.GetByAttemptAndQuestionAsync(request.QuizAttemptId, request.QuizQuestionId, cancellationToken);
        if (existing is not null)
        {
            existing.SelectedOptionId = question.QuestionType == QuestionType.Essay ? null : request.SelectedOptionId;
            existing.TextAnswer = question.QuestionType == QuestionType.Essay ? request.TextAnswer : null;
            existing.IsCorrect = isCorrect;

            await _answerRepository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<QuizAnswerDto>.Success(Map(existing));
        }

        var entity = new QuizAnswer
        {
            Id = Guid.NewGuid(),
            QuizAttemptId = request.QuizAttemptId,
            QuizQuestionId = request.QuizQuestionId,
            SelectedOptionId = question.QuestionType == QuestionType.Essay ? null : request.SelectedOptionId,
            TextAnswer = question.QuestionType == QuestionType.Essay ? request.TextAnswer : null,
            IsCorrect = isCorrect,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _answerRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizAnswerDto>.Success(Map(entity));
    }

    public async Task<Result<QuizAnswerDto>> GradeAsync(Guid id, bool isCorrect, CancellationToken cancellationToken = default)
    {
        var entity = await _answerRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.NotFound", "Quiz answer not found."));

        var question = await _questionRepository.GetByIdAsync(entity.QuizQuestionId, cancellationToken);
        if (question is not null && question.QuestionType != QuestionType.Essay)
            return Result<QuizAnswerDto>.Failure(new Error("QuizAnswer.NotManuallyGradable", "Only essay answers can be manually graded."));

        entity.IsCorrect = isCorrect;
        await _answerRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizAnswerDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _answerRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("QuizAnswer.NotFound", "Quiz answer not found."));

        var attempt = await _attemptRepository.GetByIdAsync(entity.QuizAttemptId, cancellationToken);
        if (attempt is not null && attempt.Status != QuizAttemptStatus.InProgress)
            return Result.Failure(new Error("QuizAttempt.NotInProgress", "Answers can only be removed while the attempt is in progress."));

        await _answerRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static QuizAnswerDto Map(QuizAnswer a) => new(
        a.Id, a.QuizAttemptId, a.QuizQuestionId, a.SelectedOptionId, a.TextAnswer, a.IsCorrect, a.IsActive, a.CreatedAt);
}
