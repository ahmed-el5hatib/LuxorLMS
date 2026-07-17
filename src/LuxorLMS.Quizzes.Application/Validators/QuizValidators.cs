using FluentValidation;
using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Domain.Enums;

namespace LuxorLMS.Quizzes.Application.Validators;

public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TimeLimitMinutes).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => !(x.AvailableFrom.HasValue && x.AvailableTo.HasValue) || x.AvailableTo!.Value > x.AvailableFrom!.Value)
            .WithMessage("AvailableTo must be after AvailableFrom.");
    }
}

public class UpdateQuizRequestValidator : AbstractValidator<UpdateQuizRequest>
{
    public UpdateQuizRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TimeLimitMinutes).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => !(x.AvailableFrom.HasValue && x.AvailableTo.HasValue) || x.AvailableTo!.Value > x.AvailableFrom!.Value)
            .WithMessage("AvailableTo must be after AvailableFrom.");
    }
}

public class CreateQuizQuestionRequestValidator : AbstractValidator<CreateQuizQuestionRequest>
{
    public CreateQuizQuestionRequestValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.QuestionType).IsInEnum();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Points).GreaterThan(0m);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class UpdateQuizQuestionRequestValidator : AbstractValidator<UpdateQuizQuestionRequest>
{
    public UpdateQuizQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionType).IsInEnum();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Points).GreaterThan(0m);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class CreateQuizOptionRequestValidator : AbstractValidator<CreateQuizOptionRequest>
{
    public CreateQuizOptionRequestValidator()
    {
        RuleFor(x => x.QuizQuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class UpdateQuizOptionRequestValidator : AbstractValidator<UpdateQuizOptionRequest>
{
    public UpdateQuizOptionRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class QuizBuilderOptionRequestValidator : AbstractValidator<QuizBuilderOptionRequest>
{
    public QuizBuilderOptionRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class QuizBuilderQuestionRequestValidator : AbstractValidator<QuizBuilderQuestionRequest>
{
    public QuizBuilderQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionType).IsInEnum();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Points).GreaterThan(0m);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.Options).SetValidator(new QuizBuilderOptionRequestValidator());

        RuleFor(x => x.Options)
            .Must(o => o != null && o.Count >= 2)
            .When(x => x.QuestionType == QuestionType.MultipleChoice)
            .WithMessage("Multiple-choice questions require at least two options.");

        RuleFor(x => x.Options)
            .Must(o => o != null && o.Count == 2)
            .When(x => x.QuestionType == QuestionType.TrueFalse)
            .WithMessage("True/false questions require exactly two options.");

        RuleFor(x => x.Options)
            .Must(o => o != null && o.Any(op => op.IsCorrect))
            .When(x => x.QuestionType == QuestionType.MultipleChoice || x.QuestionType == QuestionType.TrueFalse)
            .WithMessage("Objective questions require at least one correct option.");

        RuleFor(x => x.Options)
            .Must(o => o == null || o.Count == 0)
            .When(x => x.QuestionType == QuestionType.Essay)
            .WithMessage("Essay questions cannot have options.");
    }
}

public class QuizBuilderRequestValidator : AbstractValidator<QuizBuilderRequest>
{
    public QuizBuilderRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TimeLimitMinutes).GreaterThan(0);
        RuleFor(x => x.Questions).NotEmpty().WithMessage("A quiz must contain at least one question.");
        RuleForEach(x => x.Questions).SetValidator(new QuizBuilderQuestionRequestValidator());
        RuleFor(x => x)
            .Must(x => !(x.AvailableFrom.HasValue && x.AvailableTo.HasValue) || x.AvailableTo!.Value > x.AvailableFrom!.Value)
            .WithMessage("AvailableTo must be after AvailableFrom.");
    }
}

public class StartAttemptRequestValidator : AbstractValidator<StartAttemptRequest>
{
    public StartAttemptRequestValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
    }
}

public class SaveAnswerRequestValidator : AbstractValidator<SaveAnswerRequest>
{
    public SaveAnswerRequestValidator()
    {
        RuleFor(x => x.QuizAttemptId).NotEmpty();
        RuleFor(x => x.QuizQuestionId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.SelectedOptionId.HasValue || !string.IsNullOrWhiteSpace(x.TextAnswer))
            .WithMessage("An answer requires either a selected option or a text answer.");
        RuleFor(x => x.TextAnswer).MaximumLength(8000);
    }
}
