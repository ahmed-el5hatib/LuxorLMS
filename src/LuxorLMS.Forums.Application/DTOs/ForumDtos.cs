using LuxorLMS.Forums.Domain.Enums;

namespace LuxorLMS.Forums.Application.DTOs;

public record ForumTopicDto(
    Guid Id,
    Guid CourseOfferingId,
    string Title,
    Guid AuthorId,
    bool IsPinned,
    bool IsLocked,
    DateTime CreatedAt,
    Guid CreatedBy
);

public record ForumPostDto(
    Guid Id,
    Guid TopicId,
    Guid? ParentPostId,
    Guid AuthorId,
    string Body,
    ForumModerationStatus ModerationStatus,
    Guid? ModeratedBy,
    DateTime CreatedAt,
    Guid CreatedBy
);

public record CreateTopicRequest(
    Guid CourseOfferingId,
    string Title,
    string InitialPostContent
);

public record CreatePostRequest(
    Guid TopicId,
    Guid? ParentPostId,
    string Body
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    DateTime? NextCursor,
    bool HasMore
);
