using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public CourseRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Courses.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Course>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
        => await _context.Courses.AsNoTracking()
            .Where(c => c.DepartmentId == departmentId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default)
        => await _context.Courses.AnyAsync(c => c.CourseCode == courseCode, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string courseCode, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.Courses.AnyAsync(c => c.CourseCode == courseCode && c.Id != excludingId, cancellationToken);

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        => await _context.Courses.AddAsync(course, cancellationToken);

    public async Task<IReadOnlyList<CoursePrerequisite>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default)
        => await _context.CoursePrerequisites.AsNoTracking()
            .Where(cp => cp.CourseId == courseId && cp.IsActive)
            .ToListAsync(cancellationToken);

    public Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Update(course);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Remove(course);
        return Task.CompletedTask;
    }
}
