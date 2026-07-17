using LuxorLMS.Forums.Domain.Entities;

namespace LuxorLMS.Forums.Domain.Interfaces;

public interface IForumTopicRepository
{
    Task<ForumTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ForumTopic>> GetByCourseOfferingAsync(Guid courseOfferingId, DateTime? cursorCreatedAt, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ForumTopic>> SearchAsync(Guid courseOfferingId, string searchTerm, DateTime? cursorCreatedAt, int pageSize = 20, CancellationToken cancellationToken = default);
    Task AddAsync(ForumTopic topic, CancellationToken cancellationToken = default);
    void Update(ForumTopic topic);
    void Delete(ForumTopic topic);
}

public interface IForumPostRepository
{
    Task<ForumPost?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ForumPost>> GetByTopicAsync(Guid topicId, DateTime? cursorCreatedAt, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ForumPost>> SearchAsync(Guid topicId, string searchTerm, DateTime? cursorCreatedAt, int pageSize = 50, CancellationToken cancellationToken = default);
    Task AddAsync(ForumPost post, CancellationToken cancellationToken = default);
    void Update(ForumPost post);
    void Delete(ForumPost post);
}

public interface IForumsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
