using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Interfaces;

public interface IGpaService
{
    Task<Result<SemesterGpaResultDto>> GetSemesterGpaAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default);
    Task<Result<GpaResultDto>> GetCumulativeGpaAsync(Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Recomputes the CGPA from published grades and writes it back to the Academic Student record.</summary>
    Task<Result<GpaResultDto>> RecalculateAndPersistCgpaAsync(Guid studentId, CancellationToken cancellationToken = default);
}
