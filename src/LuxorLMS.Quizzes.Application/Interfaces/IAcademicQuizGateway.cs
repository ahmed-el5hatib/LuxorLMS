namespace LuxorLMS.Quizzes.Application.Interfaces;

/// <summary>
/// Anti-corruption gateway over the Academic module. Lets the Quizzes module read
/// course-offering catalog data without depending directly on Academic's internal
/// repositories from the service layer.
/// </summary>
public record CourseOfferingInfo(Guid CourseOfferingId, Guid CourseId, Guid SemesterId);

public interface IAcademicQuizGateway
{
    Task<CourseOfferingInfo?> GetCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
}
