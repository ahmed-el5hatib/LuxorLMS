namespace LuxorLMS.Attendance.Domain.Enums;

public enum AttendanceStatus
{
    Present = 1,
    Late = 2,
    Absent = 3,
    Excused = 4
}

public enum AttendanceSessionType
{
    InPerson = 1,
    Online = 2,
    Hybrid = 3
}
