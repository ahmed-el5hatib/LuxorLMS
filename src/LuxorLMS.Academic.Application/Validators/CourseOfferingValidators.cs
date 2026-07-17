using LuxorLMS.Academic.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Academic.Application.Validators;

public class CreateCourseOfferingRequestValidator : AbstractValidator<CreateCourseOfferingRequest>
{
    public CreateCourseOfferingRequestValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SemesterId).NotEmpty();
        RuleFor(x => x.PrimaryTeacherId).NotEmpty();
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.RegistrationStart).NotEmpty();
        RuleFor(x => x.RegistrationEnd).NotEmpty();
        RuleFor(x => x).Must(x => x.RegistrationEnd >= x.RegistrationStart).WithMessage("RegistrationEnd must be after or equal to RegistrationStart.");
    }
}

public class UpdateCourseOfferingRequestValidator : AbstractValidator<UpdateCourseOfferingRequest>
{
    public UpdateCourseOfferingRequestValidator()
    {
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.RegistrationStart).NotEmpty();
        RuleFor(x => x.RegistrationEnd).NotEmpty();
        RuleFor(x => x).Must(x => x.RegistrationEnd >= x.RegistrationStart).WithMessage("RegistrationEnd must be after or equal to RegistrationStart.");
    }
}

public class CreateSectionRequestValidator : AbstractValidator<CreateSectionRequest>
{
    public CreateSectionRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.AssignedStaffId).NotEmpty();
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.SectionType).IsInEnum();
    }
}

public class UpdateSectionRequestValidator : AbstractValidator<UpdateSectionRequest>
{
    public UpdateSectionRequestValidator()
    {
        RuleFor(x => x.AssignedStaffId).NotEmpty();
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.SectionType).IsInEnum();
    }
}

public class AddSectionMemberRequestValidator : AbstractValidator<AddSectionMemberRequest>
{
    public AddSectionMemberRequestValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
