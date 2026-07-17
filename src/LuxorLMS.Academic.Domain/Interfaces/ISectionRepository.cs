using LuxorLMS.Academic.Domain.Entities;

namespace LuxorLMS.Academic.Domain.Interfaces;

public interface ISectionRepository
{
    Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Section>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<int> CountMembersAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task AddAsync(Section section, CancellationToken cancellationToken = default);
    Task UpdateAsync(Section section, CancellationToken cancellationToken = default);
    Task DeleteAsync(Section section, CancellationToken cancellationToken = default);
}

public interface ISectionMemberRepository
{
    Task<SectionMember?> GetAsync(Guid sectionId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SectionMember>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task AddAsync(SectionMember member, CancellationToken cancellationToken = default);
    Task DeleteAsync(SectionMember member, CancellationToken cancellationToken = default);
}
