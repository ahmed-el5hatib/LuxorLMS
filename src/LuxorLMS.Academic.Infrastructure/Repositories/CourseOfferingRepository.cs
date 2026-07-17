using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class CourseOfferingRepository : ICourseOfferingRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public CourseOfferingRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.CourseOfferings.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<CourseOffering>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.CourseOfferings.AsNoTracking().Where(co => co.SemesterId == semesterId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        => await _context.CourseOfferings.AsNoTracking().Where(co => co.CourseId == courseId).ToListAsync(cancellationToken);

    public async Task<int> CountEnrolledAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.SectionMembers.AsNoTracking()
            .Where(sm => sm.Section!.CourseOfferingId == courseOfferingId)
            .CountAsync(cancellationToken);

    public async Task AddAsync(CourseOffering offering, CancellationToken cancellationToken = default)
        => await _context.CourseOfferings.AddAsync(offering, cancellationToken);

    public Task UpdateAsync(CourseOffering offering, CancellationToken cancellationToken = default)
    {
        _context.CourseOfferings.Update(offering);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CourseOffering offering, CancellationToken cancellationToken = default)
    {
        _context.CourseOfferings.Remove(offering);
        return Task.CompletedTask;
    }
}
