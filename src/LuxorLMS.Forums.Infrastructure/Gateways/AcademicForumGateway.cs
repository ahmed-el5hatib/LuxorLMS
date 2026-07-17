using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Forums.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Forums.Infrastructure.Gateways;

public class AcademicForumGateway : IAcademicForumGateway
{
    private readonly LuxorLMSAcademicDbContext _academicContext;

    public AcademicForumGateway(LuxorLMSAcademicDbContext academicContext)
    {
        _academicContext = academicContext;
    }

    public async Task<bool> CanAccessCourseOfferingAsync(Guid userId, Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        // Instructor/Doctor check
        var isInstructor = await _academicContext.CourseOfferings
            .AnyAsync(co => co.Id == courseOfferingId && co.PrimaryTeacherId == userId, cancellationToken);
        if (isInstructor) return true;

        // Student section membership check
        var sectionIds = await _academicContext.Sections
            .Where(s => s.CourseOfferingId == courseOfferingId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var studentId = await _academicContext.Students
            .Where(st => st.UserId == userId)
            .Select(st => (Guid?)st.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == null) return false;

        var isEnrolled = await _academicContext.SectionMembers
            .AnyAsync(sm => sectionIds.Contains(sm.SectionId) && sm.StudentId == studentId.Value, cancellationToken);

        return isEnrolled;
    }

    public async Task<bool> IsCourseInstructorOrTAAsync(Guid userId, Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var isInstructor = await _academicContext.CourseOfferings
            .AnyAsync(co => co.Id == courseOfferingId && co.PrimaryTeacherId == userId, cancellationToken);
        if (isInstructor) return true;

        var sectionIds = await _academicContext.Sections
            .Where(s => s.CourseOfferingId == courseOfferingId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var isTA = await _academicContext.Sections
            .AnyAsync(s => sectionIds.Contains(s.Id) && s.AssignedStaffId == userId, cancellationToken);

        return isTA;
    }
}
