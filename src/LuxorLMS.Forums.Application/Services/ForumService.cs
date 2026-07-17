using LuxorLMS.Forums.Application.DTOs;
using LuxorLMS.Forums.Application.Interfaces;
using LuxorLMS.Forums.Domain.Entities;
using LuxorLMS.Forums.Domain.Enums;
using LuxorLMS.Forums.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Forums.Application.Services;

public class ForumService : IForumService
{
    private readonly IForumTopicRepository _topicRepository;
    private readonly IForumPostRepository _postRepository;
    private readonly IAcademicForumGateway _academicGateway;
    private readonly IForumsUnitOfWork _unitOfWork;
    private readonly IForumsAuditLogger _auditLogger;
    private readonly IForumsNotifier _notifier;

    public ForumService(
        IForumTopicRepository topicRepository,
        IForumPostRepository postRepository,
        IAcademicForumGateway academicGateway,
        IForumsUnitOfWork unitOfWork,
        IForumsAuditLogger auditLogger,
        IForumsNotifier notifier)
    {
        _topicRepository = topicRepository;
        _postRepository = postRepository;
        _academicGateway = academicGateway;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _notifier = notifier;
    }

    public async Task<Result<ForumTopicDto>> CreateTopicAsync(CreateTopicRequest request, Guid authorId, CancellationToken cancellationToken = default)
    {
        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(authorId, request.CourseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<ForumTopicDto>.Failure(new Error("Forums.Forbidden", "User does not have access to this course offering."));

        var topic = new ForumTopic
        {
            CourseOfferingId = request.CourseOfferingId,
            Title = request.Title,
            AuthorId = authorId,
            IsPinned = false,
            IsLocked = false,
            CreatedBy = authorId,
            CreatedAt = DateTime.UtcNow
        };

        await _topicRepository.AddAsync(topic, cancellationToken);

        var firstPost = new ForumPost
        {
            TopicId = topic.Id,
            ParentPostId = null,
            AuthorId = authorId,
            Body = request.InitialPostContent,
            CreatedBy = authorId,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddAsync(firstPost, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(authorId, "Create", "ForumTopic", topic.Id.ToString(), newValues: $"{{\"Title\":\"{topic.Title}\"}}", cancellationToken: cancellationToken);
        await _notifier.NotifyNewTopicAsync(request.CourseOfferingId, topic.Id, authorId, cancellationToken);

        return Result<ForumTopicDto>.Success(MapTopic(topic));
    }

    public async Task<Result<ForumTopicDto>> GetTopicByIdAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result<ForumTopicDto>.Failure(new Error("Forums.NotFound", "Topic not found."));

        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<ForumTopicDto>.Failure(new Error("Forums.Forbidden", "Access denied to this topic."));

        return Result<ForumTopicDto>.Success(MapTopic(topic));
    }

    public async Task<Result<PagedResult<ForumTopicDto>>> GetTopicsByCourseOfferingAsync(Guid courseOfferingId, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default)
    {
        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(userId, courseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<PagedResult<ForumTopicDto>>.Failure(new Error("Forums.Forbidden", "Access denied to this course offering."));

        var topics = await _topicRepository.GetByCourseOfferingAsync(courseOfferingId, cursor, pageSize + 1, cancellationToken);
        bool hasMore = topics.Count > pageSize;
        var pageItems = topics.Take(pageSize).Select(MapTopic).ToList();
        var nextCursor = hasMore ? pageItems.Last().CreatedAt : (DateTime?)null;

        return Result<PagedResult<ForumTopicDto>>.Success(new PagedResult<ForumTopicDto>(pageItems, nextCursor, hasMore));
    }

    public async Task<Result<PagedResult<ForumTopicDto>>> SearchTopicsAsync(Guid courseOfferingId, string searchTerm, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default)
    {
        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(userId, courseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<PagedResult<ForumTopicDto>>.Failure(new Error("Forums.Forbidden", "Access denied to this course offering."));

        var topics = await _topicRepository.SearchAsync(courseOfferingId, searchTerm, cursor, pageSize + 1, cancellationToken);
        bool hasMore = topics.Count > pageSize;
        var pageItems = topics.Take(pageSize).Select(MapTopic).ToList();
        var nextCursor = hasMore ? pageItems.Last().CreatedAt : (DateTime?)null;

        return Result<PagedResult<ForumTopicDto>>.Success(new PagedResult<ForumTopicDto>(pageItems, nextCursor, hasMore));
    }

    public async Task<Result> TogglePinAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result.Failure(new Error("Forums.NotFound", "Topic not found."));

        var isStaff = await _academicGateway.IsCourseInstructorOrTAAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!isStaff)
            return Result.Failure(new Error("Forums.Forbidden", "Only Doctor/TA can pin topics."));

        var oldValue = topic.IsPinned;
        topic.IsPinned = !topic.IsPinned;
        _topicRepository.Update(topic);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "TogglePin", "ForumTopic", topic.Id.ToString(), oldValues: oldValue.ToString(), newValues: topic.IsPinned.ToString(), cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleLockAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result.Failure(new Error("Forums.NotFound", "Topic not found."));

        var isStaff = await _academicGateway.IsCourseInstructorOrTAAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!isStaff)
            return Result.Failure(new Error("Forums.Forbidden", "Only Doctor/TA can lock topics."));

        var oldValue = topic.IsLocked;
        topic.IsLocked = !topic.IsLocked;
        _topicRepository.Update(topic);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "ToggleLock", "ForumTopic", topic.Id.ToString(), oldValues: oldValue.ToString(), newValues: topic.IsLocked.ToString(), cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteTopicAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result.Failure(new Error("Forums.NotFound", "Topic not found."));

        var isStaff = await _academicGateway.IsCourseInstructorOrTAAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!isStaff && topic.AuthorId != userId)
            return Result.Failure(new Error("Forums.Forbidden", "Not authorized to delete topic."));

        _topicRepository.Delete(topic);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "Delete", "ForumTopic", topic.Id.ToString(), cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ForumPostDto>> CreatePostAsync(CreatePostRequest request, Guid authorId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
            return Result<ForumPostDto>.Failure(new Error("Forums.NotFound", "Topic not found."));

        if (topic.IsLocked)
            return Result<ForumPostDto>.Failure(new Error("Forums.TopicLocked", "Cannot post to a locked topic."));

        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(authorId, topic.CourseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<ForumPostDto>.Failure(new Error("Forums.Forbidden", "Access denied to this course offering."));

        if (request.ParentPostId.HasValue)
        {
            var parentPost = await _postRepository.GetByIdAsync(request.ParentPostId.Value, cancellationToken);
            if (parentPost is null || parentPost.TopicId != request.TopicId)
                return Result<ForumPostDto>.Failure(new Error("Forums.InvalidParent", "Referenced parent post not found in this topic."));
        }

        var post = new ForumPost
        {
            TopicId = request.TopicId,
            ParentPostId = request.ParentPostId,
            AuthorId = authorId,
            Body = request.Body,
            ModerationStatus = ForumModerationStatus.None,
            CreatedBy = authorId,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(authorId, "Create", "ForumPost", post.Id.ToString(), newValues: $"{{\"TopicId\":\"{post.TopicId}\"}}", cancellationToken: cancellationToken);
        await _notifier.NotifyNewPostAsync(request.TopicId, post.Id, authorId, cancellationToken);

        return Result<ForumPostDto>.Success(MapPost(post));
    }

    public async Task<Result<PagedResult<ForumPostDto>>> GetPostsByTopicAsync(Guid topicId, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result<PagedResult<ForumPostDto>>.Failure(new Error("Forums.NotFound", "Topic not found."));

        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<PagedResult<ForumPostDto>>.Failure(new Error("Forums.Forbidden", "Access denied."));

        var posts = await _postRepository.GetByTopicAsync(topicId, cursor, pageSize + 1, cancellationToken);
        bool hasMore = posts.Count > pageSize;
        var pageItems = posts.Take(pageSize).Select(MapPost).ToList();
        var nextCursor = hasMore ? pageItems.Last().CreatedAt : (DateTime?)null;

        return Result<PagedResult<ForumPostDto>>.Success(new PagedResult<ForumPostDto>(pageItems, nextCursor, hasMore));
    }

    public async Task<Result<PagedResult<ForumPostDto>>> SearchPostsAsync(Guid topicId, string searchTerm, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
            return Result<PagedResult<ForumPostDto>>.Failure(new Error("Forums.NotFound", "Topic not found."));

        var canAccess = await _academicGateway.CanAccessCourseOfferingAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!canAccess)
            return Result<PagedResult<ForumPostDto>>.Failure(new Error("Forums.Forbidden", "Access denied."));

        var posts = await _postRepository.SearchAsync(topicId, searchTerm, cursor, pageSize + 1, cancellationToken);
        bool hasMore = posts.Count > pageSize;
        var pageItems = posts.Take(pageSize).Select(MapPost).ToList();
        var nextCursor = hasMore ? pageItems.Last().CreatedAt : (DateTime?)null;

        return Result<PagedResult<ForumPostDto>>.Success(new PagedResult<ForumPostDto>(pageItems, nextCursor, hasMore));
    }

    public async Task<Result> ModeratePostAsync(Guid postId, ForumModerationStatus status, Guid moderatorUserId, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post is null)
            return Result.Failure(new Error("Forums.NotFound", "Post not found."));

        var topic = await _topicRepository.GetByIdAsync(post.TopicId, cancellationToken);
        if (topic is null)
            return Result.Failure(new Error("Forums.NotFound", "Topic not found."));

        var isStaff = await _academicGateway.IsCourseInstructorOrTAAsync(moderatorUserId, topic.CourseOfferingId, cancellationToken);
        if (!isStaff)
            return Result.Failure(new Error("Forums.Forbidden", "Only Doctor/TA can moderate posts."));

        var oldStatus = post.ModerationStatus;
        post.ModerationStatus = status;
        post.ModeratedBy = moderatorUserId;
        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(moderatorUserId, "Moderate", "ForumPost", post.Id.ToString(), oldValues: oldStatus.ToString(), newValues: status.ToString(), cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeletePostAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post is null)
            return Result.Failure(new Error("Forums.NotFound", "Post not found."));

        var topic = await _topicRepository.GetByIdAsync(post.TopicId, cancellationToken);
        if (topic is null)
            return Result.Failure(new Error("Forums.NotFound", "Topic not found."));

        var isStaff = await _academicGateway.IsCourseInstructorOrTAAsync(userId, topic.CourseOfferingId, cancellationToken);
        if (!isStaff && post.AuthorId != userId)
            return Result.Failure(new Error("Forums.Forbidden", "Not authorized to delete post."));

        _postRepository.Delete(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(userId, "Delete", "ForumPost", post.Id.ToString(), cancellationToken: cancellationToken);

        return Result.Success();
    }

    private static ForumTopicDto MapTopic(ForumTopic t) => new(
        t.Id,
        t.CourseOfferingId,
        t.Title,
        t.AuthorId,
        t.IsPinned,
        t.IsLocked,
        t.CreatedAt,
        t.CreatedBy
    );

    private static ForumPostDto MapPost(ForumPost p) => new(
        p.Id,
        p.TopicId,
        p.ParentPostId,
        p.AuthorId,
        p.ModerationStatus == ForumModerationStatus.Removed ? "[This post has been removed by moderator]" : p.Body,
        p.ModerationStatus,
        p.ModeratedBy,
        p.CreatedAt,
        p.CreatedBy
    );
}
