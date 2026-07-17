using AcademicInterfaces = LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Attendance.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Attendance.Infrastructure.Adapters;

/// <summary>
/// Anti-corruption adapter over the Academic module. Reads course offering, section and
/// student catalog data via Academic application services / repositories so the Attendance
/// module can validate references and resolve enrolled students.
/// </summary>
public class AcademicAttendanceGateway : IAcademicAttendanceGateway
{
    private readonly AcademicInterfaces.ICourseOfferingService _offeringService;
    private readonly ISectionRepository _sectionRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly LuxorLMSAcademicDbContext _academicContext;

    public AcademicAttendanceGateway(
        AcademicInterfaces.ICourseOfferingService offeringService,
        ISectionRepository sectionRepository,
        IStudentRepository studentRepository,
        LuxorLMSAcademicDbContext academicContext)
    {
        _offeringService = offeringService;
        _sectionRepository = sectionRepository;
        _studentRepository = studentRepository;
        _academicContext = academicContext;
    }

    public async Task<bool> CourseOfferingExistsAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var offering = await _offeringService.GetByIdAsync(courseOfferingId, cancellationToken);
        return offering.IsSuccess && offering.Value is not null;
    }

    public async Task<bool> SectionExistsAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId, cancellationToken);
        return section is not null;
    }

    public async Task<bool> StudentExistsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        return student is not null;
    }

    public async Task<IReadOnlyList<Guid>> GetEnrolledStudentIdsAsync(Guid courseOfferingId, Guid? sectionId, CancellationToken cancellationToken = default)
    {
        var sectionIds = sectionId.HasValue
            ? new List<Guid> { sectionId.Value }
            : (await _sectionRepository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken))
                .Select(s => s.Id).ToList();

        if (sectionIds.Count == 0) return new List<Guid>();

        var studentIds = await _academicContext.SectionMembers
            .AsNoTracking()
            .Where(sm => sectionIds.Contains(sm.SectionId))
            .Select(sm => sm.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return studentIds;
    }
}
