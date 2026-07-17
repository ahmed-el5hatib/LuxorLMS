using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Identity.Infrastructure.Persistence;
using LuxorLMS.Notifications.Application.DTOs;
using LuxorLMS.Notifications.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Notifications.Infrastructure.Gateways;

public class UserNotificationGateway : IUserNotificationGateway
{
    private readonly LuxorLMSIdentityDbContext _identityContext;
    private readonly LuxorLMSAcademicDbContext _academicContext;

    public UserNotificationGateway(
        LuxorLMSIdentityDbContext identityContext,
        LuxorLMSAcademicDbContext academicContext)
    {
        _identityContext = identityContext;
        _academicContext = academicContext;
    }

    public async Task<UserContactInfo?> GetContactInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _identityContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return null;

        return new UserContactInfo(user.Id, user.Email, null, null);
    }

    public async Task<IReadOnlyList<UserContactInfo>> GetCourseOfferingRecipientsAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var sectionIds = await _academicContext.Sections
            .Where(s => s.CourseOfferingId == courseOfferingId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var studentIds = await _academicContext.SectionMembers
            .Where(sm => sectionIds.Contains(sm.SectionId))
            .Select(sm => sm.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var studentUserIds = await _academicContext.Students
            .Where(st => studentIds.Contains(st.Id))
            .Select(st => st.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var users = await _identityContext.Users
            .Where(u => studentUserIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserContactInfo(u.Id, u.Email, null, null)).ToList();
    }
}
