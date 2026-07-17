using LuxorLMS.Forums.Domain.Enums;

namespace LuxorLMS.Forums.Domain.Entities;

public class ForumTopic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseOfferingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public List<ForumPost> Posts { get; set; } = new();
}

public class ForumPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TopicId { get; set; }
    public Guid? ParentPostId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; } = string.Empty;
    public ForumModerationStatus ModerationStatus { get; set; } = ForumModerationStatus.None;
    public Guid? ModeratedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ForumTopic? Topic { get; set; }
    public ForumPost? ParentPost { get; set; }
    public List<ForumPost> Replies { get; set; } = new();
}
