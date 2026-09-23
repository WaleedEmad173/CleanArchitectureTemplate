using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(ResetPasswordRequestDto Request) : IRequest<bool>;
}
