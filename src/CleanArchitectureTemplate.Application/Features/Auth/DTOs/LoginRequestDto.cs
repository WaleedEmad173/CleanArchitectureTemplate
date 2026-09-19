namespace CleanArchitectureTemplate.Application.Features.Auth.DTOs;

public sealed record LoginRequestDto(
    string Email,
    string Password);
