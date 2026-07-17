using AcademicInterfaces = LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Quizzes.Application.Interfaces;

namespace LuxorLMS.Quizzes.Infrastructure.Adapters;

/// <summary>
/// Anti-corruption adapter over the Academic module. Reads course-offering catalog data
/// via Academic application services so the Quizzes module never depends directly on
/// Academic's internal repositories from its service layer.
/// </summary>
public class AcademicQuizGateway : IAcademicQuizGateway
{
    private readonly AcademicInterfaces.ICourseOfferingService _offeringService;

    public AcademicQuizGateway(AcademicInterfaces.ICourseOfferingService offeringService)
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
