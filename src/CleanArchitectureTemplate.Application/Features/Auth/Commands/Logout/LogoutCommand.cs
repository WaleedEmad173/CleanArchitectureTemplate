using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(LogoutRequestDto Request) : IRequest<bool>;
