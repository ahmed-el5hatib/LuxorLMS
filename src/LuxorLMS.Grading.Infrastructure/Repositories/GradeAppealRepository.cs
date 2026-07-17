using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Grading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Grading.Infrastructure.Repositories;

public class GradeAppealRepository : IGradeAppealRepository
{
    private readonly LuxorLMSGradingDbContext _context;

    public GradeAppealRepository(LuxorLMSGradingDbContext context)
    {
        _context = context;
    }

    public async Task<GradeAppeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.GradeAppeals.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<GradeAppeal>> GetByStudentGradeIdAsync(Guid studentGradeId, CancellationToken cancellationToken = default)
        => await _context.GradeAppeals.AsNoTracking()
            .Where(ga => ga.StudentGradeId == studentGradeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<GradeAppeal>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.GradeAppeals.AsNoTracking()
            .Where(ga => ga.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(GradeAppeal appeal, CancellationToken cancellationToken = default)
        => await _context.GradeAppeals.AddAsync(appeal, cancellationToken);

    public Task UpdateAsync(GradeAppeal appeal, CancellationToken cancellationToken = default)
    {
        _context.GradeAppeals.Update(appeal);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(GradeAppeal appeal, CancellationToken cancellationToken = default)
    {
        _context.GradeAppeals.Remove(appeal);
        return Task.CompletedTask;
    }
}
