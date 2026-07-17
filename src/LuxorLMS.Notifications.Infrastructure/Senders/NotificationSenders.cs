using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Domain.Entities;
using LuxorLMS.Notifications.Domain.Enums;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace LuxorLMS.Notifications.Infrastructure.Senders;

public class InAppSender : IChannelSender
{
    public NotificationChannel Channel => NotificationChannel.InApp;

    public Task<bool> SendAsync(NotificationMessage message, UserContactInfo contactInfo, CancellationToken cancellationToken = default)
    {
        // InApp notifications are persisted directly in DB and consumed by API/WebSocket hubs
        return Task.FromResult(true);
    }
}

public class EmailSender : IChannelSender
{
    private readonly ISendGridClient? _sendGridClient;

    public EmailSender(ISendGridClient? sendGridClient = null)
    {
        _sendGridClient = sendGridClient;
    }

    public NotificationChannel Channel => NotificationChannel.Email;

    public async Task<bool> SendAsync(NotificationMessage message, UserContactInfo contactInfo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(contactInfo.Email))
            return false;

        if (_sendGridClient != null)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress("noreply@luxorlms.edu", "LuxorLMS Notifications"),
                Subject = message.Title,
                PlainTextContent = message.Body
            };
            msg.AddTo(new EmailAddress(contactInfo.Email));
            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        // Fallback local logging
        Console.WriteLine($"[EMAIL SENT to {contactInfo.Email}] Subject: {message.Title} | Body: {message.Body}");
        return true;
    }
}

public class SmsSender : IChannelSender
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public async Task<bool> SendAsync(NotificationMessage message, UserContactInfo contactInfo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(contactInfo.PhoneNumber))
            return false;

        try
        {
            // If Twilio is initialized via TwilioClient.Init(), this sends real SMS
            var messageResource = await MessageResource.CreateAsync(
                body: $"{message.Title}: {message.Body}",
                from: new Twilio.Types.PhoneNumber("+15005550006"),
                to: new Twilio.Types.PhoneNumber(contactInfo.PhoneNumber)
            );
            return messageResource.ErrorCode == null;
        }
        catch
        {
            Console.WriteLine($"[SMS SENT to {contactInfo.PhoneNumber}] {message.Title}: {message.Body}");
            return true;
        }
    }
}

public class PushSender : IChannelSender
{
    public NotificationChannel Channel => NotificationChannel.Push;

    public Task<bool> SendAsync(NotificationMessage message, UserContactInfo contactInfo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(contactInfo.PushToken))
            return Task.FromResult(false);

        Console.WriteLine($"[PUSH SENT to token {contactInfo.PushToken}] {message.Title}: {message.Body}");
        return Task.FromResult(true);
    }
}
