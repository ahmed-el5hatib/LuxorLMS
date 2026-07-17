using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Domain.Interfaces;

namespace LuxorLMS.Notifications.Infrastructure.Jobs;

public class SendNotificationJob
{
    private readonly INotificationService _notificationService;
    private readonly INotificationMessageRepository _messageRepository;

    public SendNotificationJob(INotificationService notificationService, INotificationMessageRepository messageRepository)
    {
        _notificationService = notificationService;
        _messageRepository = messageRepository;
    }

    public async Task ExecuteAsync(Guid messageId)
    {
        await _notificationService.DispatchMessageAsync(messageId);
    }

    public async Task ProcessScheduledJobsAsync()
    {
        var pending = await _messageRepository.GetPendingScheduledAsync(DateTime.UtcNow);
        foreach (var message in pending)
        {
            await _notificationService.DispatchMessageAsync(message.Id);
        }
    }
}
