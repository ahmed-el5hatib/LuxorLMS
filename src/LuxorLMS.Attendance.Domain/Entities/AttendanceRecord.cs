using LuxorLMS.Attendance.Domain.Enums;

namespace LuxorLMS.Attendance.Domain.Entities;

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid AttendanceSessionId { get; set; }
    public AttendanceSession? AttendanceSession { get; set; }
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public double? CheckedInLatitude { get; set; }
    public double? CheckedInLongitude { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
