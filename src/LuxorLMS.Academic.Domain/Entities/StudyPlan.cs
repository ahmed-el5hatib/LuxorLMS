namespace LuxorLMS.Academic.Domain.Entities;

public class StudyPlan
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public string VersionCode { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int MinimumCredits { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Program? Program { get; set; }
    public ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
}
