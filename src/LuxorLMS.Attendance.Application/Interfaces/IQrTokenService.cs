using LuxorLMS.Kernel;

namespace LuxorLMS.Attendance.Application.Interfaces;

public interface IQrTokenService
{
    string GenerateToken(Guid attendanceSessionId, DateTime expiresAt);
    string ComputeTokenHash(string token);
    bool ValidateToken(string token, string expectedTokenHash, DateTime expiresAt);
}
