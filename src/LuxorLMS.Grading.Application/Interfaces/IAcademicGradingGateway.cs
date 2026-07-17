namespace LuxorLMS.Grading.Application.Interfaces;

/// <summary>
/// Anti-corruption gateway over the Academic module. Lets the Grading module read
/// course/offering data and write back the computed CGPA to a student's record
/// without depending directly on Academic's internal repositories from the service layer.
/// </summary>
public record CourseCatalogInfo(Guid CourseId, int CreditHours);

public record CourseOfferingInfo(Guid CourseOfferingId, Guid CourseId, Guid SemesterId);

public interface IAcademicGradingGateway
{
    Task<CourseCatalogInfo?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseOfferingInfo?> GetCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task UpdateStudentCgpaAsync(Guid studentUserId, decimal cgpa, CancellationToken cancellationToken = default);
}
