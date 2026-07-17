namespace LuxorLMS.Registration.Application.Interfaces;

public record CourseCatalogInfo(Guid CourseId, int CreditHours, IReadOnlyList<Guid> PrerequisiteCourseIds);

public interface ICourseCatalogService
{
    Task<CourseCatalogInfo?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
