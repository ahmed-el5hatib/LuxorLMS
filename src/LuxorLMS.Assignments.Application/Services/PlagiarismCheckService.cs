using LuxorLMS.Assignments.Application.DTOs;
using LuxorLMS.Assignments.Application.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Assignments.Application.Services;

/// <summary>
/// Placeholder adapter for an external plagiarism-detection provider. Concrete
/// implementations are expected to call out to a third-party service (e.g. Turnitin);
/// this placeholder short-circuits with a null report so the submission pipeline can
/// run without a real integration wired up.
/// </summary>
public class PlagiarismCheckService : IPlagiarismCheckService
{
    public Task<Result<PlagiarismReportDto?>> CheckAsync(Guid submissionId, CancellationToken cancellationToken = default)
        => Task.FromResult(Result<PlagiarismReportDto?>.Success(null));
}
