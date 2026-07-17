using LuxorLMS.Registration.Domain.Enums;

namespace LuxorLMS.Registration.Domain.Entities;

public class CourseEnrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid SemesterId { get; set; }
    public Guid? RegistrationPeriodId { get; set; }
    public EnrollmentType EnrollmentType { get; set; } = EnrollmentType.Regular;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public int CreditHours { get; set; }
    public string? GradeLetter { get; set; }
    public bool IsPublished { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? WithdrawnAt { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
