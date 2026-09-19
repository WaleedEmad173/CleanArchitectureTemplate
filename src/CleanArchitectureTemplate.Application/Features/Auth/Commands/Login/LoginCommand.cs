using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginRequestDto Request)
    : IRequest<AuthResponseDto>;
