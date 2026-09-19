namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs;

public sealed record UserProfileDto(
    int Id,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles);
