using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Registration.Application.Interfaces;

namespace LuxorLMS.Registration.Infrastructure.Adapters;

public class CourseCatalogAdapter : ICourseCatalogService
{
    private readonly ICourseService _courseService;

    public CourseCatalogAdapter(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<CourseCatalogInfo?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await _courseService.GetByIdAsync(courseId, cancellationToken);
        if (course.IsFailure) return null;

        var prerequisites = await _courseService.GetPrerequisitesAsync(courseId, cancellationToken);
        var prerequisiteIds = prerequisites.IsSuccess && prerequisites.Value is not null
            ? prerequisites.Value.Select(p => p.PrerequisiteCourseId).ToList()
            : new List<Guid>();

        return new CourseCatalogInfo(course.Value!.Id, course.Value.CreditHours, prerequisiteIds);
    }
}
