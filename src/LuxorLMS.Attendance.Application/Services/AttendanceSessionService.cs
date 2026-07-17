using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Services;

public class AttendanceSessionService : IAttendanceSessionService
{
    private readonly IAttendanceSessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAcademicAttendanceGateway _academic;
    private readonly IQrTokenService _qrTokenService;

    public AttendanceSessionService(
        IAttendanceSessionRepository repository,
        IUnitOfWork unitOfWork,
        IAcademicAttendanceGateway academic,
        IQrTokenService qrTokenService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _academic = academic;
        _qrTokenService = qrTokenService;
    }

    public async Task<Result<IReadOnlyList<AttendanceSessionDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<AttendanceSessionDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<AttendanceSessionDto>>> GetBySectionAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetBySectionIdAsync(sectionId, cancellationToken);
        return Result<IReadOnlyList<AttendanceSessionDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<AttendanceSessionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AttendanceSessionDto>.Failure(new Error("AttendanceSession.NotFound", "Attendance session not found."));
        return Result<AttendanceSessionDto>.Success(Map(entity));
    }

    public async Task<Result<AttendanceSessionDto>> CreateAsync(CreateAttendanceSessionRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (!await _academic.CourseOfferingExistsAsync(request.CourseOfferingId, cancellationToken))
            return Result<AttendanceSessionDto>.Failure(new Error("AttendanceSession.InvalidOffering", "Referenced course offering does not exist."));

        if (request.SectionId.HasValue && !await _academic.SectionExistsAsync(request.SectionId.Value, cancellationToken))
            return Result<AttendanceSessionDto>.Failure(new Error("AttendanceSession.InvalidSection", "Referenced section does not exist."));

        if (request.TokenValidityMinutes <= 0)
            return Result<AttendanceSessionDto>.Failure(new Error("AttendanceSession.InvalidValidity", "Token validity minutes must be greater than zero."));

        var expiresAt = DateTime.UtcNow.AddMinutes(request.TokenValidityMinutes);
        var token = _qrTokenService.GenerateToken(Guid.NewGuid(), expiresAt);
        var tokenHash = _qrTokenService.ComputeTokenHash(token);

        var entity = new AttendanceSession
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            SectionId = request.SectionId,
            SessionDate = request.SessionDate,
            SessionType = request.SessionType,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            GeofenceRadiusMeters = request.GeofenceRadiusMeters,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AttendanceSessionDto>.Success(Map(entity));
    }

    public async Task<Result<AttendanceSessionDto>> UpdateAsync(Guid id, UpdateAttendanceSessionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<AttendanceSessionDto>.Failure(new Error("AttendanceSession.NotFound", "Attendance session not found."));

        entity.SessionDate = request.SessionDate;
        entity.SessionType = request.SessionType;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.GeofenceRadiusMeters = request.GeofenceRadiusMeters;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AttendanceSessionDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("AttendanceSession.NotFound", "Attendance session not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static AttendanceSessionDto Map(AttendanceSession s) => new(
        s.Id, s.CourseOfferingId, s.SectionId, s.SessionDate, s.SessionType, s.ExpiresAt,
        s.Latitude, s.Longitude, s.GeofenceRadiusMeters, s.IsActive, s.CreatedAt, s.CreatedBy);
}
