using FluentValidation;
using LuxorLMS.Grading.Application.DTOs;

namespace LuxorLMS.Grading.Application.Validators;

public class EnterGradeRequestValidator : AbstractValidator<EnterGradeRequest>
{
    public EnterGradeRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SemesterId).NotEmpty();
        RuleFor(x => x.RawScore).InclusiveBetween(0m, 100m);
    }
}

public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequest>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.RawScore).InclusiveBetween(0m, 100m);
    }
}

public class CreateGradeAppealRequestValidator : AbstractValidator<CreateGradeAppealRequest>
{
    public CreateGradeAppealRequestValidator()
    {
        RuleFor(x => x.StudentGradeId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}

public class ResolveGradeAppealRequestValidator : AbstractValidator<ResolveGradeAppealRequest>
{
    public ResolveGradeAppealRequestValidator()
    {
        RuleFor(x => x.Resolution).MaximumLength(2000);
    }
}
