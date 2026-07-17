using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Interfaces;
using LuxorLMS.Attendance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Attendance.Infrastructure.Repositories;

public class AttendanceSessionRepository : IAttendanceSessionRepository
{
    private readonly LuxorLMSAttendanceDbContext _context;

    public AttendanceSessionRepository(LuxorLMSAttendanceDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<AttendanceSession>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions.AsNoTracking()
            .Where(s => s.CourseOfferingId == courseOfferingId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AttendanceSession>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions.AsNoTracking()
            .Where(s => s.SectionId == sectionId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AttendanceSession session, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions.AddAsync(session, cancellationToken);

    public Task UpdateAsync(AttendanceSession session, CancellationToken cancellationToken = default)
    {
        _context.AttendanceSessions.Update(session);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AttendanceSession session, CancellationToken cancellationToken = default)
    {
        _context.AttendanceSessions.Remove(session);
        return Task.CompletedTask;
    }
}
