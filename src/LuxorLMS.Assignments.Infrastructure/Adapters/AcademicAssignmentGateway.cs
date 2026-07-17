using AcademicInterfaces = LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Assignments.Application.Interfaces;

namespace LuxorLMS.Assignments.Infrastructure.Adapters;

/// <summary>
/// Anti-corruption adapter over the Academic module. Reads course offering catalog data
/// via Academic application services so the Assignments module can validate that an
/// assignment references a real offering without depending on Academic's repositories.
/// </summary>
public class AcademicAssignmentGateway : IAcademicAssignmentGateway
{
    private readonly AcademicInterfaces.ICourseOfferingService _offeringService;

    public AcademicAssignmentGateway(AcademicInterfaces.ICourseOfferingService offeringService)
    {
        _offeringService = offeringService;
    }

    public async Task<CourseOfferingInfo?> GetCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var offering = await _offeringService.GetByIdAsync(courseOfferingId, cancellationToken);
        if (offering.IsFailure || offering.Value is null) return null;
        return new CourseOfferingInfo(offering.Value.Id, offering.Value.CourseId, offering.Value.SemesterId);
    }
}
