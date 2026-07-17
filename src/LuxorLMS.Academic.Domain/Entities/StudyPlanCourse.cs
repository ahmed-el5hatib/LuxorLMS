namespace LuxorLMS.Academic.Domain.Entities;

public class StudyPlanCourse
{
    public Guid Id { get; set; }
    public Guid StudyPlanId { get; set; }
    public Guid CourseId { get; set; }
    public int SemesterNumber { get; set; }
    public bool IsRequired { get; set; }
    public DateTime CreatedAt { get; set; }

    public StudyPlan? StudyPlan { get; set; }
    public Course? Course { get; set; }
}
