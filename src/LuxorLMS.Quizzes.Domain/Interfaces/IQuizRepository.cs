using LuxorLMS.Quizzes.Domain.Entities;

namespace LuxorLMS.Quizzes.Domain.Interfaces;

public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quiz>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quiz>> GetPublishedByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default);
    Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default);
    Task DeleteAsync(Quiz quiz, CancellationToken cancellationToken = default);
}
