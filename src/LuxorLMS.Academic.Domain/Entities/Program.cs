using LuxorLMS.Academic.Domain.Enums;

namespace LuxorLMS.Academic.Domain.Entities;

public class Program
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DegreeLevel DegreeLevel { get; set; }
    public int TotalCreditsRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Department? Department { get; set; }
    public ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
