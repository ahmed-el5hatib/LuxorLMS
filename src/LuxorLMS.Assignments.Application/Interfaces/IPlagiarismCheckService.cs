using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Interfaces;

/// <summary>
/// Placeholder adapter for an external plagiarism-detection provider. The concrete
/// implementation is expected to call out to a third-party service (e.g. Turnitin);
/// this placeholder returns a null report so the submission pipeline can run without
/// a real integration wired up.
/// </summary>
public interface IPlagiarismCheckService
{
    Task<Result<PlagiarismReportDto?>> CheckAsync(Guid submissionId, CancellationToken cancellationToken = default);
}
