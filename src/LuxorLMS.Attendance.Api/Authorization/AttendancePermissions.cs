namespace LuxorLMS.Attendance.Api.Authorization;

public static class AttendancePermissions
{
    public const string SessionCreate = "attendance:session:create";
    public const string SessionView = "attendance:session:view";
    public const string Mark = "attendance:mark";
    public const string AlertView = "attendance:alert:view";
    public const string AlertManage = "attendance:alert:manage";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [SessionCreate] = "Create attendance sessions (teacher)",
        [SessionView] = "View attendance sessions",
        [Mark] = "Mark attendance (student/teacher check-in)",
        [AlertView] = "View attendance alerts",
        [AlertManage] = "Manage attendance alerts"
    };
}
