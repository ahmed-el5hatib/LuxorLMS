using LuxorLMS.Kernel;
using LuxorLMS.Forums.Application.DTOs;
using LuxorLMS.Forums.Domain.Enums;

namespace LuxorLMS.Forums.Application.Interfaces;

public interface IAcademicForumGateway
{
    Task<bool> CanAccessCourseOfferingAsync(Guid userId, Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<bool> IsCourseInstructorOrTAAsync(Guid userId, Guid courseOfferingId, CancellationToken cancellationToken = default);
}

public interface IForumsAuditLogger
{
    Task LogAsync(Guid userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default);
}

public interface IForumsNotifier
{
    Task NotifyNewTopicAsync(Guid courseOfferingId, Guid topicId, Guid authorId, CancellationToken cancellationToken = default);
    Task NotifyNewPostAsync(Guid topicId, Guid postId, Guid authorId, CancellationToken cancellationToken = default);
}

public interface IForumService
{
    Task<Result<ForumTopicDto>> CreateTopicAsync(CreateTopicRequest request, Guid authorId, CancellationToken cancellationToken = default);
    Task<Result<ForumTopicDto>> GetTopicByIdAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ForumTopicDto>>> GetTopicsByCourseOfferingAsync(Guid courseOfferingId, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ForumTopicDto>>> SearchTopicsAsync(Guid courseOfferingId, string searchTerm, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> TogglePinAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ToggleLockAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteTopicAsync(Guid topicId, Guid userId, CancellationToken cancellationToken = default);

    Task<Result<ForumPostDto>> CreatePostAsync(CreatePostRequest request, Guid authorId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ForumPostDto>>> GetPostsByTopicAsync(Guid topicId, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ForumPostDto>>> SearchPostsAsync(Guid topicId, string searchTerm, DateTime? cursor, int pageSize, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ModeratePostAsync(Guid postId, ForumModerationStatus status, Guid moderatorUserId, CancellationToken cancellationToken = default);
    Task<Result> DeletePostAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);
}
