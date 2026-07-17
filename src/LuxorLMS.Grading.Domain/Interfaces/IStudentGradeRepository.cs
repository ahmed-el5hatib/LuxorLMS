using LuxorLMS.Grading.Domain.Entities;

namespace LuxorLMS.Grading.Domain.Interfaces;

public interface IStudentGradeRepository
{
    Task<StudentGrade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentGrade?> GetByOfferingAndStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentGrade>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentGrade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentGrade>> GetPublishedByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentGrade>> GetPublishedByStudentAndSemesterAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default);
    Task AddAsync(StudentGrade grade, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentGrade grade, CancellationToken cancellationToken = default);
    Task DeleteAsync(StudentGrade grade, CancellationToken cancellationToken = default);
}
