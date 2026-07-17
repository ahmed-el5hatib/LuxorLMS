using LuxorLMS.Registration.Domain.Enums;

namespace LuxorLMS.Registration.Domain.Entities;

public class StudentProgramEnrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public StudentProgramStatus Status { get; set; } = StudentProgramStatus.Active;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
