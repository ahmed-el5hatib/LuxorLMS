namespace LuxorLMS.Assignments.Application.Interfaces;

/// <summary>
/// Anti-corruption gateway over the Academic module. Lets the Assignments module read
/// course/offering catalog data (to validate an assignment belongs to a real offering)
/// without depending directly on Academic's internal repositories from the service layer.
/// </summary>
public record CourseOfferingInfo(Guid CourseOfferingId, Guid CourseId, Guid SemesterId);

public interface IAcademicAssignmentGateway
{
    Task<CourseOfferingInfo?> GetCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
}
