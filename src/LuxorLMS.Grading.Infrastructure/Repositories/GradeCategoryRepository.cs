using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Grading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Grading.Infrastructure.Repositories;

public class GradeCategoryRepository : IGradeCategoryRepository
{
    private readonly LuxorLMSGradingDbContext _context;

    public GradeCategoryRepository(LuxorLMSGradingDbContext context)
    {
        _context = context;
    }

    public async Task<GradeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.GradeCategories.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<GradeCategory>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.GradeCategories.AsNoTracking()
            .Where(gc => gc.CourseOfferingId == courseOfferingId)
            .OrderBy(gc => gc.DisplayOrder)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(GradeCategory category, CancellationToken cancellationToken = default)
        => await _context.GradeCategories.AddAsync(category, cancellationToken);

    public Task UpdateAsync(GradeCategory category, CancellationToken cancellationToken = default)
    {
        _context.GradeCategories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(GradeCategory category, CancellationToken cancellationToken = default)
    {
        _context.GradeCategories.Remove(category);
        return Task.CompletedTask;
    }
}
