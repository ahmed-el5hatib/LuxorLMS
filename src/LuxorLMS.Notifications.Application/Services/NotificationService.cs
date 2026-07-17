using LuxorLMS.Kernel;
using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Domain.Entities;
using LuxorLMS.Notifications.Domain.Enums;
using LuxorLMS.Notifications.Domain.Interfaces;

namespace LuxorLMS.Notifications.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationMessageRepository _messageRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly IUserNotificationGateway _userGateway;
    private readonly IEnumerable<IChannelSender> _senders;
    private readonly INotificationsUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationMessageRepository messageRepository,
        INotificationTemplateRepository templateRepository,
        INotificationPreferenceRepository preferenceRepository,
        IUserNotificationGateway userGateway,
        IEnumerable<IChannelSender> senders,
        INotificationsUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _templateRepository = templateRepository;
        _preferenceRepository = preferenceRepository;
        _userGateway = userGateway;
        _senders = senders;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationMessageDto>> SendNowAsync(SendNotificationRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var (title, body, templateId) = await RenderTemplateAsync(request.TemplateCode, request.Channel, request.Title, request.Body, request.Placeholders, cancellationToken);

        var message = new NotificationMessage
        {
            RecipientUserId = request.RecipientUserId,
            Channel = request.Channel,
            Title = title,
            Body = body,
            TemplateId = templateId,
            Status = NotificationStatus.Queued,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Immediate dispatch
        await DispatchMessageAsync(message.Id, cancellationToken);

        return Result<NotificationMessageDto>.Success(Map(message));
    }

    public async Task<Result<NotificationMessageDto>> ScheduleAsync(ScheduleNotificationRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var (title, body, templateId) = await RenderTemplateAsync(request.TemplateCode, request.Channel, request.Title, request.Body, request.Placeholders, cancellationToken);

        var message = new NotificationMessage
        {
            RecipientUserId = request.RecipientUserId,
            Channel = request.Channel,
            Title = title,
            Body = body,
            TemplateId = templateId,
            Status = NotificationStatus.Queued,
            ScheduledAt = request.ScheduledAt,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<NotificationMessageDto>.Success(Map(message));
    }

    public async Task<Result<int>> SendToCourseOfferingAsync(string templateCode, Guid courseOfferingId, Dictionary<string, string> placeholders, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var recipients = await _userGateway.GetCourseOfferingRecipientsAsync(courseOfferingId, cancellationToken);
        int count = 0;

        foreach (var recipient in recipients)
        {
            var (title, body, templateId) = await RenderTemplateAsync(templateCode, NotificationChannel.InApp, "Course Announcement", "You have a new announcement.", placeholders, cancellationToken);

            var message = new NotificationMessage
            {
                RecipientUserId = recipient.UserId,
                Channel = NotificationChannel.InApp,
                Title = title,
                Body = body,
                TemplateId = templateId,
                Status = NotificationStatus.Queued,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message, cancellationToken);
            count++;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(count);
    }

    public async Task<Result<IReadOnlyList<NotificationMessageDto>>> GetUserNotificationsAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetByRecipientIdAsync(userId, limit, cancellationToken);
        return Result<IReadOnlyList<NotificationMessageDto>>.Success(messages.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<NotificationPreferenceDto>>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var preferences = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
        var dtos = preferences.Select(p => new NotificationPreferenceDto(p.Id, p.UserId, p.Channel, p.Enabled)).ToList();
        return Result<IReadOnlyList<NotificationPreferenceDto>>.Success(dtos);
    }

    public async Task<Result> UpdatePreferenceAsync(Guid userId, NotificationChannel channel, bool enabled, CancellationToken cancellationToken = default)
    {
        var existing = await _preferenceRepository.GetByUserAndChannelAsync(userId, channel, cancellationToken);
        if (existing is null)
        {
            existing = new NotificationPreference
            {
                UserId = userId,
                Channel = channel,
                Enabled = enabled
            };
            await _preferenceRepository.AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.Enabled = enabled;
            _preferenceRepository.Update(existing);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task DispatchMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        if (message is null || message.Status == NotificationStatus.Sent || message.Status == NotificationStatus.Cancelled)
            return;

        var contactInfo = await _userGateway.GetContactInfoAsync(message.RecipientUserId, cancellationToken);
        if (contactInfo is null)
        {
            message.Status = NotificationStatus.Failed;
            message.Error = "Recipient contact info not found.";
            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        // Check user preferences
        var pref = await _preferenceRepository.GetByUserAndChannelAsync(message.RecipientUserId, message.Channel, cancellationToken);
        var isChannelEnabled = pref?.Enabled ?? true;

        var targetChannel = message.Channel;
        // Fallback rule: Push fail/disabled -> Email
        if (targetChannel == NotificationChannel.Push && !isChannelEnabled)
        {
            targetChannel = NotificationChannel.Email;
        }

        var sender = _senders.FirstOrDefault(s => s.Channel == targetChannel);
        if (sender is null)
        {
            message.Status = NotificationStatus.Failed;
            message.Error = $"No channel sender registered for {targetChannel}.";
            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        bool success = await sender.SendAsync(message, contactInfo, cancellationToken);

        // Fallback execution if Push failed
        if (!success && message.Channel == NotificationChannel.Push)
        {
            var emailSender = _senders.FirstOrDefault(s => s.Channel == NotificationChannel.Email);
            if (emailSender != null && !string.IsNullOrWhiteSpace(contactInfo.Email))
            {
                success = await emailSender.SendAsync(message, contactInfo, cancellationToken);
                if (success) targetChannel = NotificationChannel.Email;
            }
        }

        if (success)
        {
            message.Status = NotificationStatus.Sent;
            message.SentAt = DateTime.UtcNow;
            message.Channel = targetChannel;
        }
        else
        {
            message.Status = NotificationStatus.Failed;
            message.Error = $"Failed to send message via {targetChannel}.";
        }

        _messageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(string title, string body, Guid? templateId)> RenderTemplateAsync(
        string? templateCode, NotificationChannel channel, string defaultTitle, string defaultBody, Dictionary<string, string>? placeholders, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(templateCode))
            return (defaultTitle, defaultBody, null);

        var template = await _templateRepository.GetByCodeAsync(templateCode, channel, cancellationToken);
        if (template is null)
            return (defaultTitle, defaultBody, null);

        var title = template.Subject;
        var body = template.BodyTemplate;

        if (placeholders != null)
        {
            foreach (var kvp in placeholders)
            {
                title = title.Replace($"{{{kvp.Key}}}", kvp.Value);
                body = body.Replace($"{{{kvp.Key}}}", kvp.Value);
            }
        }

        return (title, body, template.Id);
    }

    private static NotificationMessageDto Map(NotificationMessage msg) => new(
        msg.Id,
        msg.TemplateId,
        msg.RecipientUserId,
        msg.Channel,
        msg.Title,
        msg.Body,
        msg.Status,
        msg.ScheduledAt,
        msg.SentAt,
        msg.Error,
        msg.CreatedAt
    );
}
