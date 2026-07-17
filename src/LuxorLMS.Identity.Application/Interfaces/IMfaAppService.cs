namespace LuxorLMS.Identity.Application.Interfaces;

using LuxorLMS.Kernel;

public interface IMfaAppService
{
    Task<Result<string>> EnableMfaAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> VerifyMfaAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    Task<Result> DisableMfaAsync(Guid userId, string password, CancellationToken cancellationToken = default);
    Task<Result> ValidateMfaCodeAsync(string mfaToken, string code, CancellationToken cancellationToken = default);
}
