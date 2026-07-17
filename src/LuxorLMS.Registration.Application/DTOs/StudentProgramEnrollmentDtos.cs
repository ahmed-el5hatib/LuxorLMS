using LuxorLMS.Registration.Domain.Enums;

namespace LuxorLMS.Registration.Application.DTOs;

public record StudentProgramEnrollmentDto(Guid Id, Guid StudentId, Guid ProgramId, DateTime EnrollmentDate, StudentProgramStatus Status, DateTime CreatedAt);
public record CreateStudentProgramEnrollmentRequest(Guid StudentId, Guid ProgramId);
public record UpdateStudentProgramEnrollmentRequest(StudentProgramStatus Status);
