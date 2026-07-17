using FluentValidation;
using LuxorLMS.Assignments.Application.DTOs;

namespace LuxorLMS.Assignments.Application.Validators;

public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequest>
{
    public CreateAssignmentRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.MaxScore).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}

public class UpdateAssignmentRequestValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.MaxScore).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}

public class CreateAssignmentRubricRequestValidator : AbstractValidator<CreateAssignmentRubricRequest>
{
    public CreateAssignmentRubricRequestValidator()
    {
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x.Criteria).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.MaxPoints).GreaterThan(0);
    }
}

public class UpdateAssignmentRubricRequestValidator : AbstractValidator<UpdateAssignmentRubricRequest>
{
    public UpdateAssignmentRubricRequestValidator()
    {
        RuleFor(x => x.Criteria).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.MaxPoints).GreaterThan(0);
    }
}

public class SubmitAssignmentRequestValidator : AbstractValidator<SubmitAssignmentRequest>
{
    public SubmitAssignmentRequestValidator()
    {
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}

public class GradeSubmissionRequestValidator : AbstractValidator<GradeSubmissionRequest>
{
    public GradeSubmissionRequestValidator()
    {
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        RuleFor(x => x.Feedback).MaximumLength(4000);
    }
}

public class ReturnSubmissionRequestValidator : AbstractValidator<ReturnSubmissionRequest>
{
    public ReturnSubmissionRequestValidator()
    {
        RuleFor(x => x.Feedback).MaximumLength(4000);
    }
}

public class AddAssignmentFileRequestValidator : AbstractValidator<AddAssignmentFileRequest>
{
    public AddAssignmentFileRequestValidator()
    {
        RuleFor(x => x.AssignmentSubmissionId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.FileUrl).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FileSizeBytes).GreaterThan(0);
    }
}
