using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public SectionRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Sections.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Section>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.Sections.AsNoTracking().Where(s => s.CourseOfferingId == courseOfferingId).ToListAsync(cancellationToken);

    public async Task<int> CountMembersAsync(Guid sectionId, CancellationToken cancellationToken = default)
        => await _context.SectionMembers.AsNoTracking().CountAsync(sm => sm.SectionId == sectionId, cancellationToken);

    public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
        => await _context.Sections.AddAsync(section, cancellationToken);

    public Task UpdateAsync(Section section, CancellationToken cancellationToken = default)
    {
        _context.Sections.Update(section);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Section section, CancellationToken cancellationToken = default)
    {
        _context.Sections.Remove(section);
        return Task.CompletedTask;
    }
}

public class SectionMemberRepository : ISectionMemberRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public SectionMemberRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<SectionMember?> GetAsync(Guid sectionId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.SectionMembers.AsNoTracking()
            .FirstOrDefaultAsync(sm => sm.SectionId == sectionId && sm.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<SectionMember>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        => await _context.SectionMembers.AsNoTracking().Where(sm => sm.SectionId == sectionId).ToListAsync(cancellationToken);

    public async Task AddAsync(SectionMember member, CancellationToken cancellationToken = default)
        => await _context.SectionMembers.AddAsync(member, cancellationToken);

    public Task DeleteAsync(SectionMember member, CancellationToken cancellationToken = default)
    {
        _context.SectionMembers.Remove(member);
        return Task.CompletedTask;
    }
}
