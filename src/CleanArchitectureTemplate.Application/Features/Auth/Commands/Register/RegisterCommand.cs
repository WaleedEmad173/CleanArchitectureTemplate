using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(RegisterRequestDto Request)
    : IRequest<AuthResponseDto>;
