using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Enums;
using LuxorLMS.Quizzes.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Quizzes.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly IQuizOptionRepository _optionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicQuizGateway _academic;

    public QuizService(
        IQuizRepository quizRepository,
        IQuizQuestionRepository questionRepository,
        IQuizOptionRepository optionRepository,
        IUnitOfWork unitOfWork,
        IAcademicQuizGateway academic)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _optionRepository = optionRepository;
        _unitOfWork = unitOfWork;
        _academic = academic;
    }

    // ---------------- Quizzes ----------------

    public async Task<Result<IReadOnlyList<QuizDto>>> GetByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _quizRepository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<QuizDto>>.Success(items.Select(MapQuiz).ToList());
    }

    public async Task<Result<QuizDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _quizRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));
        return Result<QuizDto>.Success(MapQuiz(entity));
    }

    public async Task<Result<QuizDetailDto>> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(id, cancellationToken);
        if (quiz is null) return Result<QuizDetailDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        var questions = await _questionRepository.GetByQuizIdAsync(id, cancellationToken);
        var questionIds = questions.Select(q => q.Id).ToList();
        var options = await _optionRepository.GetByQuestionIdsAsync(questionIds, cancellationToken);

        var builderQuestions = questions
            .OrderBy(q => q.DisplayOrder)
            .Select(q => new QuizBuilderQuestionDto(
                MapQuestion(q),
                options.Where(o => o.QuizQuestionId == q.Id)
                       .OrderBy(o => o.DisplayOrder)
                       .Select(MapOption)
                       .ToList()))
            .ToList();

        return Result<QuizDetailDto>.Success(new QuizDetailDto(MapQuiz(quiz), builderQuestions));
    }

    public async Task<Result<QuizDto>> CreateAsync(CreateQuizRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var validation = ValidateQuizWindow(request.TimeLimitMinutes, request.AvailableFrom, request.AvailableTo);
        if (validation.IsError) return Result<QuizDto>.Failure(validation);

        var offering = await _academic.GetCourseOfferingAsync(request.CourseOfferingId, cancellationToken);
        if (offering is null)
            return Result<QuizDto>.Failure(new Error("Quiz.InvalidOffering", "Referenced course offering does not exist."));

        var entity = new Quiz
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            Title = request.Title,
            Description = request.Description,
            TimeLimitMinutes = request.TimeLimitMinutes,
            IsPublished = false,
            AvailableFrom = request.AvailableFrom,
            AvailableTo = request.AvailableTo,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _quizRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizDto>.Success(MapQuiz(entity));
    }

    public async Task<Result<QuizDto>> UpdateAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _quizRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        if (entity.IsPublished)
            return Result<QuizDto>.Failure(new Error("Quiz.AlreadyPublished", "Published quizzes cannot be edited; unpublish first."));

        var validation = ValidateQuizWindow(request.TimeLimitMinutes, request.AvailableFrom, request.AvailableTo);
        if (validation.IsError) return Result<QuizDto>.Failure(validation);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.TimeLimitMinutes = request.TimeLimitMinutes;
        entity.AvailableFrom = request.AvailableFrom;
        entity.AvailableTo = request.AvailableTo;
        entity.IsActive = request.IsActive;

        await _quizRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizDto>.Success(MapQuiz(entity));
    }

    public async Task<Result<QuizDto>> PublishAsync(Guid id, bool publish, CancellationToken cancellationToken = default)
    {
        var entity = await _quizRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizDto>.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        if (publish)
        {
            var questions = await _questionRepository.GetByQuizIdAsync(id, cancellationToken);
            if (!questions.Any(q => q.IsActive))
                return Result<QuizDto>.Failure(new Error("Quiz.NoQuestions", "A quiz must have at least one active question before publishing."));
        }

        entity.IsPublished = publish;
        await _quizRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizDto>.Success(MapQuiz(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _quizRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Quiz.NotFound", "Quiz not found."));

        if (entity.IsPublished)
            return Result.Failure(new Error("Quiz.AlreadyPublished", "Published quizzes cannot be deleted; unpublish first."));

        await _quizRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ---------------- Visual builder ----------------

    public async Task<Result<QuizDetailDto>> BuildAsync(QuizBuilderRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var validation = ValidateQuizWindow(request.TimeLimitMinutes, request.AvailableFrom, request.AvailableTo);
        if (validation.IsError) return Result<QuizDetailDto>.Failure(validation);

        if (request.Questions is null || request.Questions.Count == 0)
            return Result<QuizDetailDto>.Failure(new Error("Quiz.NoQuestions", "A quiz must contain at least one question."));

        var offering = await _academic.GetCourseOfferingAsync(request.CourseOfferingId, cancellationToken);
        if (offering is null)
            return Result<QuizDetailDto>.Failure(new Error("Quiz.InvalidOffering", "Referenced course offering does not exist."));

        // Validate each question's structure before persisting.
        foreach (var q in request.Questions)
        {
            var structure = ValidateBuilderQuestion(q);
            if (structure.IsError) return Result<QuizDetailDto>.Failure(structure);
        }

        var now = DateTime.UtcNow;
        var quiz = new Quiz
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            Title = request.Title,
            Description = request.Description,
            TimeLimitMinutes = request.TimeLimitMinutes,
            IsPublished = false,
            AvailableFrom = request.AvailableFrom,
            AvailableTo = request.AvailableTo,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = createdBy
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _quizRepository.AddAsync(quiz, cancellationToken);

            foreach (var q in request.Questions)
            {
                var question = new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    QuizId = quiz.Id,
                    QuestionType = q.QuestionType,
                    Text = q.Text,
                    Points = q.Points,
                    DisplayOrder = q.DisplayOrder,
                    IsActive = true,
                    CreatedAt = now,
                    CreatedBy = createdBy
                };
                await _questionRepository.AddAsync(question, cancellationToken);

                if (q.Options is not null)
                {
                    foreach (var o in q.Options)
                    {
                        var option = new QuizOption
                        {
                            Id = Guid.NewGuid(),
                            QuizQuestionId = question.Id,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect,
                            DisplayOrder = o.DisplayOrder,
                            IsActive = true,
                            CreatedAt = now,
                            CreatedBy = createdBy
                        };
                        await _optionRepository.AddAsync(option, cancellationToken);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return await GetDetailAsync(quiz.Id, cancellationToken);
    }

    // ---------------- Questions ----------------

    public async Task<Result<IReadOnlyList<QuizQuestionDto>>> GetQuestionsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        var items = await _questionRepository.GetByQuizIdAsync(quizId, cancellationToken);
        return Result<IReadOnlyList<QuizQuestionDto>>.Success(items.Select(MapQuestion).ToList());
    }

    public async Task<Result<QuizQuestionDto>> GetQuestionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _questionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizQuestionDto>.Failure(new Error("QuizQuestion.NotFound", "Quiz question not found."));
        return Result<QuizQuestionDto>.Success(MapQuestion(entity));
    }

    public async Task<Result<QuizQuestionDto>> CreateQuestionAsync(CreateQuizQuestionRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz is null) return Result<QuizQuestionDto>.Failure(new Error("QuizQuestion.InvalidQuiz", "Referenced quiz does not exist."));

        if (quiz.IsPublished)
            return Result<QuizQuestionDto>.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify questions of a published quiz."));

        if (request.Points <= 0)
            return Result<QuizQuestionDto>.Failure(new Error("QuizQuestion.InvalidPoints", "Points must be greater than 0."));

        var entity = new QuizQuestion
        {
            Id = Guid.NewGuid(),
            QuizId = request.QuizId,
            QuestionType = request.QuestionType,
            Text = request.Text,
            Points = request.Points,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _questionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizQuestionDto>.Success(MapQuestion(entity));
    }

    public async Task<Result<QuizQuestionDto>> UpdateQuestionAsync(Guid id, UpdateQuizQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizQuestionDto>.Failure(new Error("QuizQuestion.NotFound", "Quiz question not found."));

        var quiz = await _quizRepository.GetByIdAsync(entity.QuizId, cancellationToken);
        if (quiz is not null && quiz.IsPublished)
            return Result<QuizQuestionDto>.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify questions of a published quiz."));

        if (request.Points <= 0)
            return Result<QuizQuestionDto>.Failure(new Error("QuizQuestion.InvalidPoints", "Points must be greater than 0."));

        entity.QuestionType = request.QuestionType;
        entity.Text = request.Text;
        entity.Points = request.Points;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;

        await _questionRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizQuestionDto>.Success(MapQuestion(entity));
    }

    public async Task<Result> DeleteQuestionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _questionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("QuizQuestion.NotFound", "Quiz question not found."));

        var quiz = await _quizRepository.GetByIdAsync(entity.QuizId, cancellationToken);
        if (quiz is not null && quiz.IsPublished)
            return Result.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify questions of a published quiz."));

        await _questionRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ---------------- Options ----------------

    public async Task<Result<IReadOnlyList<QuizOptionDto>>> GetOptionsByQuestionAsync(Guid quizQuestionId, CancellationToken cancellationToken = default)
    {
        var items = await _optionRepository.GetByQuestionIdAsync(quizQuestionId, cancellationToken);
        return Result<IReadOnlyList<QuizOptionDto>>.Success(items.Select(MapOption).ToList());
    }

    public async Task<Result<QuizOptionDto>> GetOptionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _optionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizOptionDto>.Failure(new Error("QuizOption.NotFound", "Quiz option not found."));
        return Result<QuizOptionDto>.Success(MapOption(entity));
    }

    public async Task<Result<QuizOptionDto>> CreateOptionAsync(CreateQuizOptionRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(request.QuizQuestionId, cancellationToken);
        if (question is null) return Result<QuizOptionDto>.Failure(new Error("QuizOption.InvalidQuestion", "Referenced quiz question does not exist."));

        if (question.QuestionType == QuestionType.Essay)
            return Result<QuizOptionDto>.Failure(new Error("QuizOption.NotAllowed", "Essay questions cannot have options."));

        var quiz = await _quizRepository.GetByIdAsync(question.QuizId, cancellationToken);
        if (quiz is not null && quiz.IsPublished)
            return Result<QuizOptionDto>.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify options of a published quiz."));

        var entity = new QuizOption
        {
            Id = Guid.NewGuid(),
            QuizQuestionId = request.QuizQuestionId,
            Text = request.Text,
            IsCorrect = request.IsCorrect,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _optionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizOptionDto>.Success(MapOption(entity));
    }

    public async Task<Result<QuizOptionDto>> UpdateOptionAsync(Guid id, UpdateQuizOptionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _optionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<QuizOptionDto>.Failure(new Error("QuizOption.NotFound", "Quiz option not found."));

        var question = await _questionRepository.GetByIdAsync(entity.QuizQuestionId, cancellationToken);
        if (question is not null)
        {
            var quiz = await _quizRepository.GetByIdAsync(question.QuizId, cancellationToken);
            if (quiz is not null && quiz.IsPublished)
                return Result<QuizOptionDto>.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify options of a published quiz."));
        }

        entity.Text = request.Text;
        entity.IsCorrect = request.IsCorrect;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;

        await _optionRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<QuizOptionDto>.Success(MapOption(entity));
    }

    public async Task<Result> DeleteOptionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _optionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("QuizOption.NotFound", "Quiz option not found."));

        var question = await _questionRepository.GetByIdAsync(entity.QuizQuestionId, cancellationToken);
        if (question is not null)
        {
            var quiz = await _quizRepository.GetByIdAsync(question.QuizId, cancellationToken);
            if (quiz is not null && quiz.IsPublished)
                return Result.Failure(new Error("Quiz.AlreadyPublished", "Cannot modify options of a published quiz."));
        }

        await _optionRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // ---------------- Helpers ----------------

    private static Error ValidateQuizWindow(int timeLimitMinutes, DateTime? availableFrom, DateTime? availableTo)
    {
        if (timeLimitMinutes <= 0)
            return new Error("Quiz.InvalidTimeLimit", "TimeLimitMinutes must be greater than 0.");

        if (availableFrom.HasValue && availableTo.HasValue && availableTo.Value <= availableFrom.Value)
            return new Error("Quiz.InvalidWindow", "AvailableTo must be after AvailableFrom.");

        return Error.None;
    }

    private static Error ValidateBuilderQuestion(QuizBuilderQuestionRequest q)
    {
        if (string.IsNullOrWhiteSpace(q.Text))
            return new Error("QuizQuestion.InvalidText", "Question text is required.");

        if (q.Points <= 0)
            return new Error("QuizQuestion.InvalidPoints", "Points must be greater than 0.");

        var optionCount = q.Options?.Count ?? 0;

        switch (q.QuestionType)
        {
            case QuestionType.MultipleChoice:
                if (optionCount < 2)
                    return new Error("QuizQuestion.InvalidOptions", "Multiple-choice questions require at least two options.");
                if (!q.Options!.Any(o => o.IsCorrect))
                    return new Error("QuizQuestion.NoCorrectOption", "Multiple-choice questions require at least one correct option.");
                break;

            case QuestionType.TrueFalse:
                if (optionCount != 2)
                    return new Error("QuizQuestion.InvalidOptions", "True/false questions require exactly two options.");
                if (q.Options!.Count(o => o.IsCorrect) != 1)
                    return new Error("QuizQuestion.NoCorrectOption", "True/false questions require exactly one correct option.");
                break;

            case QuestionType.Essay:
                if (optionCount > 0)
                    return new Error("QuizQuestion.InvalidOptions", "Essay questions cannot have options.");
                break;
        }

        return Error.None;
    }

    private static QuizDto MapQuiz(Quiz q) => new(
        q.Id, q.CourseOfferingId, q.Title, q.Description, q.TimeLimitMinutes,
        q.IsPublished, q.AvailableFrom, q.AvailableTo, q.IsActive, q.CreatedAt);

    private static QuizQuestionDto MapQuestion(QuizQuestion q) => new(
        q.Id, q.QuizId, q.QuestionType, q.Text, q.Points, q.DisplayOrder, q.IsActive, q.CreatedAt);

    private static QuizOptionDto MapOption(QuizOption o) => new(
        o.Id, o.QuizQuestionId, o.Text, o.IsCorrect, o.DisplayOrder, o.IsActive, o.CreatedAt);
}
