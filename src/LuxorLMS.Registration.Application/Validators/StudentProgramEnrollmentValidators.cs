using LuxorLMS.Registration.Application.DTOs;
using FluentValidation;

namespace LuxorLMS.Registration.Application.Validators;

public class CreateStudentProgramEnrollmentRequestValidator : AbstractValidator<CreateStudentProgramEnrollmentRequest>
{
    public CreateStudentProgramEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.ProgramId).NotEmpty();
    }
}
