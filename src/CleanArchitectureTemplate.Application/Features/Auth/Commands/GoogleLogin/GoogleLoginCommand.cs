using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin
{
    public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponseDto>;
}
