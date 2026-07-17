using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Enums;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly IQuizAnswerRepository _answerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuizAttemptService(
        IQuizAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        IQuizQuestionRepository questionRepository,
        IQuizAnswerRepository answerRepository,
        IUnitOfWork unitOfWork)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<QuizAttemptDto>>> GetByQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        var items = await _attemptRepository.GetByQuizIdAsync(quizId, cancellationToken);
        return Result<IReadOnlyList<QuizAttemptDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<QuizAttemptDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _attemptRepository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<QuizAttemptDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<QuizAttemptDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _attemptRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizAttemptDto>.Failure(new Error("QuizAttempt.NotFound", "Quiz attempt not found."));
        return Result<QuizAttemptDto>.Success(Map(entity));
    }

    public async Task<Result<QuizAttemptDto>> StartAsync(Guid quizId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId, cancellationToken);
        if (quiz is null) return Result<QuizAttemptDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        if (!quiz.IsPublished || !quiz.IsActive)
            return Result<QuizAttemptDto>.Failure(new Error("Quiz.NotAvailable", "Quiz is not published or is inactive."));

        var now = DateTime.UtcNow;
        if (quiz.AvailableFrom.HasValue && now < quiz.AvailableFrom.Value)
            return Result<QuizAttemptDto>.Failure(new Error("Quiz.NotOpenYet", "Quiz is not yet available."));
        if (quiz.AvailableTo.HasValue && now > quiz.AvailableTo.Value)
            return Result<QuizAttemptDto>.Failure(new Error("Quiz.Closed", "Quiz availability window has closed."));

        var inProgress = await _attemptRepository.GetInProgressAsync(quizId, studentId, cancellationToken);
        if (inProgress is not null)
        {
            // Auto-expire the previous attempt if its time is already up, otherwise resume it.
            if (IsExpired(inProgress, quiz.TimeLimitMinutes, now))
            {
                await ExpireAsync(inProgress, quiz, cancellationToken);
            }
            else
            {
                return Result<QuizAttemptDto>.Success(Map(inProgress));
            }
        }

        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            StudentId = studentId,
            StartedAt = now,
            SubmittedAt = null,
            Score = null,
            Status = QuizAttemptStatus.InProgress,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = studentId
        };

        await _attemptRepository.AddAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizAttemptDto>.Success(Map(attempt));
    }

    public async Task<Result<QuizAttemptDto>> SubmitAsync(Guid attemptId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId, cancellationToken);
        if (attempt is null) return Result<QuizAttemptDto>.Failure(new Error("QuizAttempt.NotFound", "Quiz attempt not found."));

        if (attempt.StudentId != studentId)
            return Result<QuizAttemptDto>.Failure(new Error("QuizAttempt.Forbidden", "This attempt does not belong to the current student."));

        if (attempt.Status != QuizAttemptStatus.InProgress)
            return Result<QuizAttemptDto>.Failure(new Error("QuizAttempt.NotInProgress", "Only in-progress attempts can be submitted."));

        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null) return Result<QuizAttemptDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        var now = DateTime.UtcNow;
        var expired = IsExpired(attempt, quiz.TimeLimitMinutes, now);

        await ScoreAsync(attempt, quiz, cancellationToken);
        attempt.SubmittedAt = now;
        attempt.Status = expired ? QuizAttemptStatus.Expired : QuizAttemptStatus.Submitted;

        await _attemptRepository.UpdateAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizAttemptDto>.Success(Map(attempt));
    }

    public async Task<Result<QuizAttemptDto>> AutoSubmitIfExpiredAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId, cancellationToken);
        if (attempt is null) return Result<QuizAttemptDto>.Failure(new Error("QuizAttempt.NotFound", "Quiz attempt not found."));

        if (attempt.Status != QuizAttemptStatus.InProgress)
            return Result<QuizAttemptDto>.Success(Map(attempt));

        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null) return Result<QuizAttemptDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        var now = DateTime.UtcNow;
        if (!IsExpired(attempt, quiz.TimeLimitMinutes, now))
            return Result<QuizAttemptDto>.Success(Map(attempt));

        await ExpireAsync(attempt, quiz, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizAttemptDto>.Success(Map(attempt));
    }

    // ---------------- Helpers ----------------

    private static bool IsExpired(QuizAttempt attempt, int timeLimitMinutes, DateTime now)
        => now >= attempt.StartedAt.AddMinutes(timeLimitMinutes);

    private async Task ExpireAsync(QuizAttempt attempt, Quiz quiz, CancellationToken cancellationToken)
    {
        await ScoreAsync(attempt, quiz, cancellationToken);
        attempt.SubmittedAt = attempt.StartedAt.AddMinutes(quiz.TimeLimitMinutes);
        attempt.Status = QuizAttemptStatus.Expired;
        await _attemptRepository.UpdateAsync(attempt, cancellationToken);
    }

    /// <summary>
    /// Computes the attempt score from graded answers. Objective questions are auto-graded when the
    /// answer is saved; if every question in the quiz is objective the attempt is marked Graded,
    /// otherwise it awaits manual essay grading.
    /// </summary>
    private async Task ScoreAsync(QuizAttempt attempt, Quiz quiz, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetByQuizIdAsync(quiz.Id, cancellationToken);
        var activeQuestions = questions.Where(q => q.IsActive).ToList();
        var answers = await _answerRepository.GetByAttemptIdAsync(attempt.Id, cancellationToken);

        decimal score = 0m;
        foreach (var answer in answers.Where(a => a.IsCorrect))
        {
            var question = activeQuestions.FirstOrDefault(q => q.Id == answer.QuizQuestionId);
            if (question is not null) score += question.Points;
        }

        attempt.Score = score;
    }

    private static QuizAttemptDto Map(QuizAttempt a) => new(
        a.Id, a.QuizId, a.StudentId, a.StartedAt, a.SubmittedAt, a.Score, a.Status, a.IsActive, a.CreatedAt);
}
