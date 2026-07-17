namespace LuxorLMS.Assignments.Domain.Entities;

public class AssignmentRubric
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string Criteria { get; set; } = string.Empty;
    public decimal MaxPoints { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
