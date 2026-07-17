using LuxorLMS.Forums.Application.Interfaces;
using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Domain.Enums;

namespace LuxorLMS.Forums.Infrastructure.Services;

public class ForumsNotifier : IForumsNotifier
{
    private readonly INotificationService _notificationService;

    public ForumsNotifier(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task NotifyNewTopicAsync(Guid courseOfferingId, Guid topicId, Guid authorId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            ["TopicId"] = topicId.ToString(),
            ["AuthorId"] = authorId.ToString()
        };

        await _notificationService.SendToCourseOfferingAsync(
            "forum.new_topic",
            courseOfferingId,
            placeholders,
            authorId,
            cancellationToken);
    }

    public async Task NotifyNewPostAsync(Guid topicId, Guid postId, Guid authorId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            ["TopicId"] = topicId.ToString(),
            ["PostId"] = postId.ToString(),
            ["AuthorId"] = authorId.ToString()
        };

        await _notificationService.SendToCourseOfferingAsync(
            "forum.new_post",
            topicId,
            placeholders,
            authorId,
            cancellationToken);
    }
}
