using FluentValidation;
using LuxorLMS.Grading.Application.DTOs;

namespace LuxorLMS.Grading.Application.Validators;

public class CreateGradeCategoryRequestValidator : AbstractValidator<CreateGradeCategoryRequest>
{
    public CreateGradeCategoryRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Weight).InclusiveBetween(0m, 1m);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class UpdateGradeCategoryRequestValidator : AbstractValidator<UpdateGradeCategoryRequest>
{
    public UpdateGradeCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Weight).InclusiveBetween(0m, 1m);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

public class CreateGradeComponentRequestValidator : AbstractValidator<CreateGradeComponentRequest>
{
    public CreateGradeComponentRequestValidator()
    {
        RuleFor(x => x.GradeCategoryId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.MaxPoints).GreaterThan(0m);
    }
}

public class UpdateGradeComponentRequestValidator : AbstractValidator<UpdateGradeComponentRequest>
{
    public UpdateGradeComponentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.MaxPoints).GreaterThan(0m);
    }
}
