using LuxorLMS.Registration.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Registration.Application.Validators;

public class RegisterCourseRequestValidator : AbstractValidator<RegisterCourseRequest>
{
    public RegisterCourseRequestValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SemesterId).NotEmpty();
    }
}
