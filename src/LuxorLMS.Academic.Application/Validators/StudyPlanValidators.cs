using LuxorLMS.Academic.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Academic.Application.Validators;

public class CreateStudyPlanRequestValidator : AbstractValidator<CreateStudyPlanRequest>
{
    public CreateStudyPlanRequestValidator()
    {
        RuleFor(x => x.ProgramId).NotEmpty();
        RuleFor(x => x.VersionCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x).Must(x => !x.EffectiveTo.HasValue || x.EffectiveTo.Value >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be greater than or equal to EffectiveFrom.");
        RuleFor(x => x.MinimumCredits).GreaterThanOrEqualTo(0);
    }
}

public class UpdateStudyPlanRequestValidator : AbstractValidator<UpdateStudyPlanRequest>
{
    public UpdateStudyPlanRequestValidator()
    {
        RuleFor(x => x.VersionCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x).Must(x => !x.EffectiveTo.HasValue || x.EffectiveTo.Value >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be greater than or equal to EffectiveFrom.");
        RuleFor(x => x.MinimumCredits).GreaterThanOrEqualTo(0);
    }
}
