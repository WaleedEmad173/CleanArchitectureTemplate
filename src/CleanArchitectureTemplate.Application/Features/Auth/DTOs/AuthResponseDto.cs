namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
