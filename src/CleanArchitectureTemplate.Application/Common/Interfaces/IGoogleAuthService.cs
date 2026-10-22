namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public sealed record GoogleUserInfo(
    string Subject,
    string Email,
    string? Name,
    bool EmailVerified);

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ValidateIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default);
}
