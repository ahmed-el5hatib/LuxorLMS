using LuxorLMS.Attendance.Application.DTOs;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Enums;
using LuxorLMS.Attendance.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Services;

public class AttendanceAlertService : IAttendanceAlertService
{
    private const double FlagThresholdPercentage = 25.0;

    private readonly IAttendanceAlertRepository _repository;

    public AttendanceAlertService(IAttendanceAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<AttendanceAlertDto>>> GetByCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);

        var grouped = records
            .GroupBy(r => r.StudentId)
            .Select(g => ComputeAlert(g.Key, courseOfferingId, g.ToList()))
            .ToList();

        return Result<IReadOnlyList<AttendanceAlertDto>>.Success(grouped);
    }

    public async Task<Result<IReadOnlyList<AttendanceAlertDto>>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByStudentIdAsync(studentId, cancellationToken);

        var grouped = records
            .GroupBy(r => r.StudentId)
            .Select(g => ComputeAlert(g.Key, Guid.Empty, g.ToList()))
            .ToList();

        return Result<IReadOnlyList<AttendanceAlertDto>>.Success(grouped);
    }

    private static AttendanceAlertDto ComputeAlert(Guid studentId, Guid courseOfferingId, IReadOnlyList<AttendanceRecord> records)
    {
        var total = records.Count;
        var absent = records.Count(r => r.Status == AttendanceStatus.Absent);
        var late = records.Count(r => r.Status == AttendanceStatus.Late);
        var present = records.Count(r => r.Status == AttendanceStatus.Present);

        var absencePercentage = total == 0 ? 0.0 : (double)(absent + late) / total * 100.0;
        var isFlagged = absencePercentage > FlagThresholdPercentage;

        return new AttendanceAlertDto(
            studentId, courseOfferingId, total, absent, late, present, Math.Round(absencePercentage, 2), isFlagged);
    }
}
