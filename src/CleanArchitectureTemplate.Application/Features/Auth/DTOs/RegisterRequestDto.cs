namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs;

public sealed record RegisterRequestDto(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword);
