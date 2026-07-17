using System.Security.Cryptography;
using System.Text;
using LuxorLMS.Attendance.Application.Interfaces;

namespace LuxorLMS.Attendance.Application.Services;

public class QrTokenService : IQrTokenService
{
    public string GenerateToken(Guid attendanceSessionId, DateTime expiresAt)
    {
        var raw = $"{attendanceSessionId:N}|{expiresAt:O}|{Guid.NewGuid():N}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    public string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public bool ValidateToken(string token, string expectedTokenHash, DateTime expiresAt)
    {
        if (DateTime.UtcNow > expiresAt) return false;
        var hash = ComputeTokenHash(token);
        return string.Equals(hash, expectedTokenHash, StringComparison.OrdinalIgnoreCase);
    }
}
