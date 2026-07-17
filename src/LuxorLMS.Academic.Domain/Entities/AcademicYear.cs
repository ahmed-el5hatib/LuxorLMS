namespace LuxorLMS.Academic.Domain.Entities;

public class AcademicYear
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
}
