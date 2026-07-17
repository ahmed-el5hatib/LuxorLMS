using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Grading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Grading.Infrastructure.Repositories;

public class GradeComponentRepository : IGradeComponentRepository
{
    private readonly LuxorLMSGradingDbContext _context;

    public GradeComponentRepository(LuxorLMSGradingDbContext context)
    {
        _context = context;
    }

    public async Task<GradeComponent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.GradeComponents.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<GradeComponent>> GetByCategoryIdAsync(Guid gradeCategoryId, CancellationToken cancellationToken = default)
        => await _context.GradeComponents.AsNoTracking()
            .Where(gc => gc.GradeCategoryId == gradeCategoryId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(GradeComponent component, CancellationToken cancellationToken = default)
        => await _context.GradeComponents.AddAsync(component, cancellationToken);

    public Task UpdateAsync(GradeComponent component, CancellationToken cancellationToken = default)
    {
        _context.GradeComponents.Update(component);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(GradeComponent component, CancellationToken cancellationToken = default)
    {
        _context.GradeComponents.Remove(component);
        return Task.CompletedTask;
    }
}
