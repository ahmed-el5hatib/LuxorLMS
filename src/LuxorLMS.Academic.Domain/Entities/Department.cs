namespace LuxorLMS.Academic.Domain.Entities;

public class Department
{
    public Guid Id { get; set; }
    public Guid FacultyId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? HeadId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public Faculty? Faculty { get; set; }
    public ICollection<Program> Programs { get; set; } = new List<Program>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
