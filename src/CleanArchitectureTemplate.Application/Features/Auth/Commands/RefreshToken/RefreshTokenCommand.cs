using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(RefreshRequestDto Request)
    : IRequest<AuthResponseDto>;
