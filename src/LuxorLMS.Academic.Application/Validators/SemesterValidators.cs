using LuxorLMS.Academic.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Academic.Application.Validators;

public class CreateSemesterRequestValidator : AbstractValidator<CreateSemesterRequest>
{
    public CreateSemesterRequestValidator()
    {
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.RegistrationStart).NotEmpty();
        RuleFor(x => x.RegistrationEnd).NotEmpty();
        RuleFor(x => x).Must(x => x.EndDate > x.StartDate)
            .WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x).Must(x => x.RegistrationEnd >= x.RegistrationStart)
            .WithMessage("RegistrationEnd must be after or equal to RegistrationStart.");
        RuleFor(x => x).Must(x => x.RegistrationStart >= x.StartDate && x.RegistrationEnd <= x.EndDate)
            .WithMessage("Registration period must fall within the semester bounds.");
    }
}

public class UpdateSemesterRequestValidator : AbstractValidator<UpdateSemesterRequest>
{
    public UpdateSemesterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.RegistrationStart).NotEmpty();
        RuleFor(x => x.RegistrationEnd).NotEmpty();
        RuleFor(x => x).Must(x => x.EndDate > x.StartDate)
            .WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x).Must(x => x.RegistrationEnd >= x.RegistrationStart)
            .WithMessage("RegistrationEnd must be after or equal to RegistrationStart.");
        RuleFor(x => x).Must(x => x.RegistrationStart >= x.StartDate && x.RegistrationEnd <= x.EndDate)
            .WithMessage("Registration period must fall within the semester bounds.");
    }
}
