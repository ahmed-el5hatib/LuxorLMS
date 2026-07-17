namespace LuxorLMS.Academic.Domain.Entities;

public class SectionMember
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Section? Section { get; set; }
}
