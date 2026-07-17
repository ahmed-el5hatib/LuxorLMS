namespace LuxorLMS.Identity.Application.Interfaces;

using LuxorLMS.Kernel;

public interface IPasswordPolicyService
{
    Task<Result> ValidateAsync(string password);
}
