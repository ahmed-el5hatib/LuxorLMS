using System.Security.Cryptography;
using System.Text;

namespace LuxorLMS.Identity.Infrastructure.Services;

public interface IMfaService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string email, string secret);
    Task<bool> ValidateAsync(string secret, string code);
}

public class MfaService : IMfaService
{
    private const int CodeDigits = 6;
    private const int CodeValidWindow = 1;

    public string GenerateSecret()
    {
        var bytes = new byte[20];
        RandomNumberGenerator.Fill(bytes);
        return Base32Encode(bytes);
    }

    public string GenerateQrCodeUri(string email, string secret)
    {
        var issuer = "LuxorLMS";
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&digits={CodeDigits}";
    }

    public async Task<bool> ValidateAsync(string secret, string code)
    {
        var secretBytes = Base32Decode(secret);
        var codeInt = int.Parse(code);

        var currentTimeStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;

        for (var i = -CodeValidWindow; i <= CodeValidWindow; i++)
        {
            var timeStep = currentTimeStep + i;
            var computedCode = ComputeHmacSha1(secretBytes, timeStep);
            if (computedCode == codeInt)
            {
                return await Task.FromResult(true);
            }
        }

        return false;
    }

    private static int ComputeHmacSha1(byte[] secret, long timeStep)
    {
        var timeStepBytes = BitConverter.GetBytes(timeStep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timeStepBytes);
        }

        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(timeStepBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode = ((hash[offset] & 0x7F) << 24) |
                         ((hash[offset + 1] & 0xFF) << 16) |
                         ((hash[offset + 2] & 0xFF) << 8) |
                         (hash[offset + 3] & 0xFF);

        return binaryCode % (int)Math.Pow(10, CodeDigits);
    }

    private static string Base32Encode(byte[] data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var sb = new StringBuilder();
        int bits = 0, value = 0;

        foreach (var b in data)
        {
            value = (value << 8) | b;
            bits += 8;

            while (bits >= 5)
            {
                sb.Append(alphabet[(value >> (bits - 5)) & 31]);
                bits -= 5;
            }
        }

        if (bits > 0)
        {
            sb.Append(alphabet[(value << (5 - bits)) & 31]);
        }

        return sb.ToString();
    }

    private static byte[] Base32Decode(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var bits = 0;
        int value = 0;
        var output = new List<byte>();

        foreach (var c in input.ToUpperInvariant())
        {
            if (c == '=') break;

            var index = alphabet.IndexOf(c);
            if (index < 0) continue;

            value = (value << 5) | index;
            bits += 5;

            if (bits >= 8)
            {
                output.Add((byte)(value >> (bits - 8)));
                bits -= 8;
            }
        }

        return output.ToArray();
    }
}
