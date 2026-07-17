namespace LuxorLMS.Academic.Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Department? Department { get; set; }
    public ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
    public ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
    public ICollection<CoursePrerequisite> DependentCourses { get; set; } = new List<CoursePrerequisite>();
}
