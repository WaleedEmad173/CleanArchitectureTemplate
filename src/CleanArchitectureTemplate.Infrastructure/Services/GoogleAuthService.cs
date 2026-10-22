using CleanArchitectureTemplate.Application.Common.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public sealed class GoogleAuthService(IConfiguration configuration) : IGoogleAuthService
{
    public async Task<GoogleUserInfo> ValidateIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        var clientId = configuration["GoogleAuth:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("GoogleAuth:ClientId is not configured.");

        cancellationToken.ThrowIfCancellationRequested();

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId]
            });

        cancellationToken.ThrowIfCancellationRequested();

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.Name,
            payload.EmailVerified);
    }
}
