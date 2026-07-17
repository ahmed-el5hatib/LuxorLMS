using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class StudyPlanRepository : IStudyPlanRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public StudyPlanRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.StudyPlans.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<StudyPlan>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
        => await _context.StudyPlans.AsNoTracking()
            .Where(sp => sp.ProgramId == programId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByVersionCodeAsync(string versionCode, CancellationToken cancellationToken = default)
        => await _context.StudyPlans.AnyAsync(sp => sp.VersionCode == versionCode, cancellationToken);

    public async Task<bool> ExistsByVersionCodeAsync(string versionCode, Guid excludingId, CancellationToken cancellationToken = default)
        => await _context.StudyPlans.AnyAsync(sp => sp.VersionCode == versionCode && sp.Id != excludingId, cancellationToken);

    public async Task AddAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default)
        => await _context.StudyPlans.AddAsync(studyPlan, cancellationToken);

    public Task UpdateAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default)
    {
        _context.StudyPlans.Update(studyPlan);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(StudyPlan studyPlan, CancellationToken cancellationToken = default)
    {
        _context.StudyPlans.Remove(studyPlan);
        return Task.CompletedTask;
    }
}
