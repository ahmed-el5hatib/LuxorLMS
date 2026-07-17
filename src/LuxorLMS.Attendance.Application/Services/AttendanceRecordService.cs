using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Enums;
using LuxorLMS.Attendance.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Services;

public class AttendanceRecordService : IAttendanceRecordService
{
    private const double EarthRadiusMeters = 6371000.0;
    private const double LateThresholdMinutes = 10.0;

    private readonly IAttendanceRecordRepository _repository;
    private readonly IAttendanceSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicAttendanceGateway _academic;
    private readonly IQrTokenService _qrTokenService;

    public AttendanceRecordService(
        IAttendanceRecordRepository repository,
        IAttendanceSessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        IAcademicAttendanceGateway academic,
        IQrTokenService qrTokenService)
    {
        _repository = repository;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _academic = academic;
        _qrTokenService = qrTokenService;
    }

    public async Task<Result<AttendanceRecordDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.NotFound", "Attendance record not found."));
        return Result<AttendanceRecordDto>.Success(Map(entity));
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetBySessionAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetBySessionIdAsync(attendanceSessionId, cancellationToken);
        return Result<IReadOnlyList<AttendanceRecordDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<AttendanceRecordDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<AttendanceRecordDto>> MarkAsync(MarkAttendanceRequest request, Guid markedBy, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(request.AttendanceSessionId, cancellationToken);
        if (session is null)
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.SessionNotFound", "Attendance session not found."));

        if (!session.IsActive)
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.SessionInactive", "Attendance session is no longer active."));

        if (DateTime.UtcNow > session.ExpiresAt)
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.TokenExpired", "The attendance token has expired."));

        if (!_qrTokenService.ValidateToken(request.Token, session.TokenHash, session.ExpiresAt))
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.InvalidToken", "The attendance token is invalid."));

        if (!await _academic.StudentExistsAsync(request.StudentId, cancellationToken))
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.StudentNotFound", "Referenced student does not exist."));

        if (session.Latitude.HasValue && session.Longitude.HasValue && session.GeofenceRadiusMeters.HasValue)
        {
            if (!request.CheckedInLatitude.HasValue || !request.CheckedInLongitude.HasValue)
                return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.MissingLocation", "Geolocation is required for this session."));

            var distance = Haversine(session.Latitude.Value, session.Longitude.Value, request.CheckedInLatitude.Value, request.CheckedInLongitude.Value);
            if (distance > session.GeofenceRadiusMeters.Value)
                return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.OutOfGeofence", "Checked-in location is outside the allowed geofence."));
        }

        var existing = await _repository.GetBySessionAndStudentAsync(session.Id, request.StudentId, cancellationToken);
        if (existing is not null)
            return Result<AttendanceRecordDto>.Failure(new Error("AttendanceRecord.Duplicate", "Attendance already recorded for this student in this session."));

        var status = (DateTime.UtcNow - session.ExpiresAt.AddMinutes(LateThresholdMinutes)) > TimeSpan.Zero
            ? AttendanceStatus.Late
            : AttendanceStatus.Present;

        var entity = new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            AttendanceSessionId = session.Id,
            StudentId = request.StudentId,
            Status = status,
            CheckedInLatitude = request.CheckedInLatitude,
            CheckedInLongitude = request.CheckedInLongitude,
            CheckedInAt = DateTime.UtcNow,
            Notes = request.Notes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = markedBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AttendanceRecordDto>.Success(Map(entity));
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

    private static AttendanceRecordDto Map(AttendanceRecord r) => new(
        r.Id, r.AttendanceSessionId, r.StudentId, r.Status,
        r.CheckedInLatitude, r.CheckedInLongitude, r.CheckedInAt, r.Notes,
        r.IsActive, r.CreatedAt, r.CreatedBy);
}
