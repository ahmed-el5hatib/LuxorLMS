namespace LuxorLMS.Registration.Domain.Entities;

public class RegistrationPeriod
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public Guid? ProgramId { get; set; }
    public Guid AcademicYearId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? LateRegistrationStart { get; set; }
    public DateTime? LateRegistrationEnd { get; set; }
    public int MinCreditHours { get; set; } = 12;
    public int MaxCreditHours { get; set; } = 18;
    public decimal GpaCapForMax { get; set; } = 2.0m;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
