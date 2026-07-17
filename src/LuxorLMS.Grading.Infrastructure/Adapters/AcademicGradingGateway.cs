using AcademicInterfaces = LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Grading.Application.Interfaces;

namespace LuxorLMS.Grading.Infrastructure.Adapters;

/// <summary>
/// Anti-corruption adapter over the Academic module. Reads course/offering catalog data
/// via Academic application services and writes the computed CGPA back to the Academic
/// Student record via the Academic student repository (same cross-module wiring the
/// Registration module uses).
/// </summary>
public class AcademicGradingGateway : IAcademicGradingGateway
{
    private readonly AcademicInterfaces.ICourseService _courseService;
    private readonly AcademicInterfaces.ICourseOfferingService _offeringService;
    private readonly IStudentRepository _studentRepository;
    private readonly LuxorLMSAcademicDbContext _academicContext;

    public AcademicGradingGateway(
        AcademicInterfaces.ICourseService courseService,
        AcademicInterfaces.ICourseOfferingService offeringService,
        IStudentRepository studentRepository,
        LuxorLMSAcademicDbContext academicContext)
    {
        _courseService = courseService;
        _offeringService = offeringService;
        _studentRepository = studentRepository;
        _academicContext = academicContext;
    }

    public async Task<CourseCatalogInfo?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await _courseService.GetByIdAsync(courseId, cancellationToken);
        if (course.IsFailure || course.Value is null) return null;
        return new CourseCatalogInfo(course.Value.Id, course.Value.CreditHours);
    }

    public async Task<CourseOfferingInfo?> GetCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var offering = await _offeringService.GetByIdAsync(courseOfferingId, cancellationToken);
        if (offering.IsFailure || offering.Value is null) return null;
        return new CourseOfferingInfo(offering.Value.Id, offering.Value.CourseId, offering.Value.SemesterId);
    }

    public async Task UpdateStudentCgpaAsync(Guid studentUserId, decimal cgpa, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(studentUserId, cancellationToken);
        if (student is null) return;

        student.Gpa = cgpa;
        await _studentRepository.UpdateAsync(student, cancellationToken);
        await _academicContext.SaveChangesAsync(cancellationToken);
    }
}
