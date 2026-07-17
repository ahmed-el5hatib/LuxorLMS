using LuxorLMS.Academic.Domain.Enums;

namespace LuxorLMS.Academic.Domain.Entities;

public class CourseOffering
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid SemesterId { get; set; }
    public Guid PrimaryTeacherId { get; set; }
    public int Capacity { get; set; }
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public OfferingStatus Status { get; set; } = OfferingStatus.Draft;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Course? Course { get; set; }
    public Semester? Semester { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
}
