using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Interfaces;
using LuxorLMS.Attendance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Attendance.Infrastructure.Repositories;

public class AttendanceRecordRepository : IAttendanceRecordRepository
{
    private readonly LuxorLMSAttendanceDbContext _context;

    public AttendanceRecordRepository(LuxorLMSAttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.FindAsync(new object[] { id }, cancellationToken);

    public async Task<AttendanceRecord?> GetBySessionAndStudentAsync(Guid attendanceSessionId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.AsNoTracking()
            .FirstOrDefaultAsync(r => r.AttendanceSessionId == attendanceSessionId && r.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<AttendanceRecord>> GetBySessionIdAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.AsNoTracking()
            .Where(r => r.AttendanceSessionId == attendanceSessionId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AttendanceRecord>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AttendanceRecord>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.AsNoTracking()
            .Where(r => r.AttendanceSession!.CourseOfferingId == courseOfferingId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default)
        => await _context.AttendanceRecords.AddAsync(record, cancellationToken);

    public Task UpdateAsync(AttendanceRecord record, CancellationToken cancellationToken = default)
    {
        _context.AttendanceRecords.Update(record);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AttendanceRecord record, CancellationToken cancellationToken = default)
    {
        _context.AttendanceRecords.Remove(record);
        return Task.CompletedTask;
    }
}
