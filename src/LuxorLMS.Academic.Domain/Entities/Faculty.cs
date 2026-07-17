namespace LuxorLMS.Academic.Domain.Entities;

public class Faculty
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
