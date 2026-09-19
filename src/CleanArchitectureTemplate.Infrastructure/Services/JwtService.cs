using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public sealed class JwtService(IConfiguration configuration) : IJwtService
{
    public Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(
        int userId,
        string email,
        string userName,
        string fullName,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        var section = configuration.GetSection("Jwt");

        var key = section["Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.TryParse(section["ExpireMinutes"], out var minutes)
                ? minutes
                : 15);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email),
            new("fullName", fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: section["Issuer"],
            audience: section["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return Task.FromResult((
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt));
    }
}
