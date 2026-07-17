using LuxorLMS.Forums.Domain.Entities;
using LuxorLMS.Forums.Domain.Interfaces;
using LuxorLMS.Forums.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Forums.Infrastructure.Repositories;

public class ForumTopicRepository : IForumTopicRepository
{
    private readonly LuxorLMSForumsDbContext _context;

    public ForumTopicRepository(LuxorLMSForumsDbContext context)
    {
        _context = context;
    }

    public async Task<ForumTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ForumTopics
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ForumTopic>> GetByCourseOfferingAsync(Guid courseOfferingId, DateTime? cursorCreatedAt, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.ForumTopics
            .Where(t => t.CourseOfferingId == courseOfferingId && !t.IsDeleted);

        if (cursorCreatedAt.HasValue)
        {
            query = query.Where(t => t.CreatedAt < cursorCreatedAt.Value);
        }

        return await query
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.CreatedAt)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ForumTopic>> SearchAsync(Guid courseOfferingId, string searchTerm, DateTime? cursorCreatedAt, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.ForumTopics
            .Where(t => t.CourseOfferingId == courseOfferingId && !t.IsDeleted)
            .Where(t => EF.Functions.ToTsVector("english", t.Title).Matches(EF.Functions.PlainToTsQuery("english", searchTerm)));

        if (cursorCreatedAt.HasValue)
        {
            query = query.Where(t => t.CreatedAt < cursorCreatedAt.Value);
        }

        return await query
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.CreatedAt)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ForumTopic topic, CancellationToken cancellationToken = default)
    {
        await _context.ForumTopics.AddAsync(topic, cancellationToken);
    }

    public void Update(ForumTopic topic)
    {
        _context.ForumTopics.Update(topic);
    }

    public void Delete(ForumTopic topic)
    {
        topic.IsDeleted = true;
        topic.DeletedAt = DateTime.UtcNow;
        _context.ForumTopics.Update(topic);
    }
}

public class ForumPostRepository : IForumPostRepository
{
    private readonly LuxorLMSForumsDbContext _context;

    public ForumPostRepository(LuxorLMSForumsDbContext context)
    {
        _context = context;
    }

    public async Task<ForumPost?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ForumPosts
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ForumPost>> GetByTopicAsync(Guid topicId, DateTime? cursorCreatedAt, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.ForumPosts
            .Where(p => p.TopicId == topicId && !p.IsDeleted);

        if (cursorCreatedAt.HasValue)
        {
            query = query.Where(p => p.CreatedAt > cursorCreatedAt.Value);
        }

        return await query
            .OrderBy(p => p.CreatedAt)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ForumPost>> SearchAsync(Guid topicId, string searchTerm, DateTime? cursorCreatedAt, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.ForumPosts
            .Where(p => p.TopicId == topicId && !p.IsDeleted)
            .Where(p => EF.Functions.ToTsVector("english", p.Body).Matches(EF.Functions.PlainToTsQuery("english", searchTerm)));

        if (cursorCreatedAt.HasValue)
        {
            query = query.Where(p => p.CreatedAt > cursorCreatedAt.Value);
        }

        return await query
            .OrderBy(p => p.CreatedAt)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ForumPost post, CancellationToken cancellationToken = default)
    {
        await _context.ForumPosts.AddAsync(post, cancellationToken);
    }

    public void Update(ForumPost post)
    {
        _context.ForumPosts.Update(post);
    }

    public void Delete(ForumPost post)
    {
        post.IsDeleted = true;
        post.DeletedAt = DateTime.UtcNow;
        _context.ForumPosts.Update(post);
    }
}

public class ForumsUnitOfWork : IForumsUnitOfWork
{
    private readonly LuxorLMSForumsDbContext _context;

    public ForumsUnitOfWork(LuxorLMSForumsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
