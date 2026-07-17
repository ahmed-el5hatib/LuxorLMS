using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Identity.Application.Services;

public class PasswordPolicyService : IPasswordPolicyService
{
    private const int MinimumLength = 10;
    private const int MaximumLength = 128;

    public Task<Result> ValidateAsync(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinimumLength)
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_TOO_SHORT", $"Password must be at least {MinimumLength} characters")));
        }

        if (password.Length > MaximumLength)
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_TOO_LONG", $"Password must not exceed {MaximumLength} characters")));
        }

        if (!password.Any(char.IsUpper))
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_NO_UPPERCASE", "Password must contain at least one uppercase letter")));
        }

        if (!password.Any(char.IsLower))
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_NO_LOWERCASE", "Password must contain at least one lowercase letter")));
        }

        if (!password.Any(char.IsDigit))
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_NO_DIGIT", "Password must contain at least one digit")));
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            return Task.FromResult(Result.Failure(new Error("PASSWORD_NO_SYMBOL", "Password must contain at least one symbol")));
        }

        return Task.FromResult(Result.Success(default));
    }
}
