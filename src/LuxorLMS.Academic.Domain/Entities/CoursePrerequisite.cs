namespace LuxorLMS.Academic.Domain.Entities;

public class CoursePrerequisite
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Course? Course { get; set; }
    public Course? PrerequisiteCourse { get; set; }
}
