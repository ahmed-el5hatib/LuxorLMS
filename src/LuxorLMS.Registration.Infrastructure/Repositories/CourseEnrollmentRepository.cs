using LuxorLMS.Registration.Domain.Enums;
using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Interfaces;
using LuxorLMS.Registration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Registration.Infrastructure.Repositories;

public class CourseEnrollmentRepository : ICourseEnrollmentRepository
{
    private readonly LuxorLMSRegistrationDbContext _context;

    public CourseEnrollmentRepository(LuxorLMSRegistrationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<CourseEnrollment?> GetByStudentCourseSemesterAsync(Guid studentId, Guid courseId, Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .FirstOrDefaultAsync(ce => ce.StudentId == studentId && ce.CourseId == courseId && ce.SemesterId == semesterId, cancellationToken);

    public async Task<IReadOnlyList<CourseEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .Where(ce => ce.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CourseEnrollment>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .Where(ce => ce.SemesterId == semesterId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CourseEnrollment>> GetApprovedByStudentAndSemesterAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .Where(ce => ce.StudentId == studentId && ce.SemesterId == semesterId && ce.Status == EnrollmentStatus.Approved)
            .ToListAsync(cancellationToken);

    public async Task<int> SumApprovedCreditHoursAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .Where(ce => ce.StudentId == studentId && ce.SemesterId == semesterId && ce.Status == EnrollmentStatus.Approved)
            .SumAsync(ce => ce.CreditHours, cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetCompletedCourseIdsAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AsNoTracking()
            .Where(ce => ce.StudentId == studentId && ce.Status == EnrollmentStatus.Approved && ce.IsPublished)
            .Select(ce => ce.CourseId)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
        => await _context.CourseEnrollments.AddAsync(enrollment, cancellationToken);

    public Task UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.CourseEnrollments.Update(enrollment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.CourseEnrollments.Remove(enrollment);
        return Task.CompletedTask;
    }
}
