namespace LuxorLMS.Reporting.Domain.Enums;

public enum ReportType
{
    Transcript = 1,
    Roster = 2,
    GradeReport = 3,
    AttendanceReport = 4,
    Certificate = 5,
    Custom = 6
}

public enum ExportFormat
{
    Pdf = 1,
    Excel = 2,
    Csv = 3
}

public enum ReportStatus
{
    Queued = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}
