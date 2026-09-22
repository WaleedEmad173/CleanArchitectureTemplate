using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgetPassword
{
    public sealed record ForgetPasswordCommand(ForgetPasswordRequestDto Request) : IRequest<bool>;
}
