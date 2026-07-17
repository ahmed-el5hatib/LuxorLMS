using LuxorLMS.Registration.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Registration.Application.Validators;

public class CreateRegistrationPeriodRequestValidator : AbstractValidator<CreateRegistrationPeriodRequest>
{
    public CreateRegistrationPeriodRequestValidator()
    {
        RuleFor(x => x.SemesterId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x).Must(x => x.EndDate > x.StartDate).WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x.MinCreditHours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxCreditHours).GreaterThan(0);
        RuleFor(x => x.MaxCreditHours).GreaterThanOrEqualTo(x => x.MinCreditHours).WithMessage("MaxCreditHours must be greater than or equal to MinCreditHours.");
        RuleFor(x => x.GpaCapForMax).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => !x.LateRegistrationStart.HasValue || !x.LateRegistrationEnd.HasValue || x.LateRegistrationEnd.Value >= x.LateRegistrationStart.Value)
            .WithMessage("LateRegistrationEnd must be after or equal to LateRegistrationStart.");
    }
}

public class UpdateRegistrationPeriodRequestValidator : AbstractValidator<UpdateRegistrationPeriodRequest>
{
    public UpdateRegistrationPeriodRequestValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x).Must(x => x.EndDate > x.StartDate).WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x.MinCreditHours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxCreditHours).GreaterThan(0);
        RuleFor(x => x.MaxCreditHours).GreaterThanOrEqualTo(x => x.MinCreditHours).WithMessage("MaxCreditHours must be greater than or equal to MinCreditHours.");
        RuleFor(x => x.GpaCapForMax).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => !x.LateRegistrationStart.HasValue || !x.LateRegistrationEnd.HasValue || x.LateRegistrationEnd.Value >= x.LateRegistrationStart.Value)
            .WithMessage("LateRegistrationEnd must be after or equal to LateRegistrationStart.");
    }
}
