using LuxorLMS.Academic.Domain.Enums;

namespace LuxorLMS.Academic.Domain.Entities;

public class Section
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public SectionType SectionType { get; set; }
    public Guid AssignedStaffId { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public CourseOffering? CourseOffering { get; set; }
    public ICollection<SectionMember> Members { get; set; } = new List<SectionMember>();
}
