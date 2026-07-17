using LuxorLMS.Attendance.Domain.Enums;

namespace LuxorLMS.Attendance.Application.DTOs;

public record AttendanceSessionDto(
    Guid Id,
    Guid CourseOfferingId,
    Guid? SectionId,
    DateTime SessionDate,
    AttendanceSessionType SessionType,
    DateTime ExpiresAt,
    double? Latitude,
    double? Longitude,
    double? GeofenceRadiusMeters,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record CreateAttendanceSessionRequest(
    Guid CourseOfferingId,
    Guid? SectionId,
    DateTime SessionDate,
    AttendanceSessionType SessionType,
    int TokenValidityMinutes,
    double? Latitude,
    double? Longitude,
    double? GeofenceRadiusMeters);

public record UpdateAttendanceSessionRequest(
    DateTime SessionDate,
    AttendanceSessionType SessionType,
    double? Latitude,
    double? Longitude,
    double? GeofenceRadiusMeters,
    bool IsActive);

public record AttendanceRecordDto(
    Guid Id,
    Guid AttendanceSessionId,
    Guid StudentId,
    AttendanceStatus Status,
    double? CheckedInLatitude,
    double? CheckedInLongitude,
    DateTime? CheckedInAt,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    Guid CreatedBy);

public record MarkAttendanceRequest(
    Guid AttendanceSessionId,
    Guid StudentId,
    string Token,
    double? CheckedInLatitude,
    double? CheckedInLongitude,
    string? Notes);

public record AttendanceAlertDto(
    Guid StudentId,
    Guid CourseOfferingId,
    int TotalSessions,
    int AbsentCount,
    int LateCount,
    int PresentCount,
    double AbsencePercentage,
    bool IsFlagged);

public record GenerateQrTokenRequest(
    Guid AttendanceSessionId,
    int ValidityMinutes);
