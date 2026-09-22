using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordRequestDto Request) : IRequest<bool>;
}
