namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(
        int userId,
        string email,
        string userName,
        string fullName,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);
}
