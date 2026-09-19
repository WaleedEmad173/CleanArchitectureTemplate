using CleanArchitectureTemplate.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public sealed class TokenGenerator : ITokenGenerator
{
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
