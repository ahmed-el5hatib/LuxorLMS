using LuxorLMS.Attendance.Domain.Enums;

namespace LuxorLMS.Attendance.Domain.Entities;

public class AttendanceSession
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public Guid? SectionId { get; set; }
    public DateTime SessionDate { get; set; }
    public AttendanceSessionType SessionType { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? GeofenceRadiusMeters { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
